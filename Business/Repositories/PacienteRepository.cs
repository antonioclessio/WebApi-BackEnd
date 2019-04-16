using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Paciente;
using GrupoOrto.ERP.Entities.Entity.Table;
using System;
using System.Linq;
using System.Collections.Generic;
using GrupoOrto.ERP.Entities.Enum;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class PacienteRepository : BaseRepository, IRepository<Paciente, PacienteListResult, PacienteDetailResult, PacienteFilterQuery, PacienteForm>
    {
        protected override int IdAplicacao { get => (int)AplicacaoEnum.Paciente; }
        readonly byte PERMISSAO_GERENCIAR_SATISFACAO;

        public PacienteRepository()
        {
            PERMISSAO_GERENCIAR_SATISFACAO = 10;
        }

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(key);
                context.Entry(entity).State = EntityState.Deleted;

                RegistrarLogAtividade(context, entity, PERMISSAO_EXCLUSAO);
                return context.SaveChanges() > 0; ;
            }
        }

        public PacienteDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from paciente in context.Paciente
                              join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                              join clinica in context.Clinica on paciente.IdClinica equals clinica.IdClinica
                              where paciente.IdClinica == IdClinicaLogada && paciente.IdPaciente == key
                              select new PacienteDetailResult
                              {
                                  IdPaciente = paciente.IdPaciente,
                                  Nome = pessoa.Nome,
                                  CPF = pessoa.CPF,
                                  Codigo = paciente.Codigo,
                                  SiglaClinica = clinica.Sigla,
                                  IdLocalizacaoGeografica = pessoa.IdLocalizacaoGeografica,
                                  IdPessoa = pessoa.IdPessoa,
                                  Status = paciente.Status,
                                  DataHoraCadastro = paciente.DataHoraCadastro,
                                  DataNascimento = pessoa.DataNascimento
                              }).FirstOrDefault();

                if (entity != null)
                {

                    if (entity.IdLocalizacaoGeografica.HasValue)
                    {
                        var geoLocRep = new LocalizacaoGeograficaRepository();
                        entity.LocalizacaoGeografica = geoLocRep.GetByKey(entity.IdLocalizacaoGeografica.Value);
                    }

                    entity.Paciente_Email.AddRange(context.Pessoa_Email.Where(a => a.IdPessoa == entity.IdPessoa).ToList());
                    entity.Paciente_Telefone.AddRange(context.Pessoa_Telefone.Where(a => a.IdPessoa == entity.IdPessoa).ToList());
                }

                return entity;
            }
        }

        public Paciente GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Paciente.FirstOrDefault(a => a.IdPaciente == key);
            }
        }

        public PacienteForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from paciente in context.Paciente
                              join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                              where paciente.IdPessoa == key
                                 && paciente.IdClinica == IdClinicaLogada
                              select new PacienteForm
                              {

                              }).FirstOrDefault();

                if (entity != null)
                {
                    if (entity.IdLocalizacaoGeografica.HasValue)
                    {
                        var geoLocRep = new LocalizacaoGeograficaRepository();
                        entity.LocalizacaoGeografica = geoLocRep.GetByKey(entity.IdLocalizacaoGeografica.Value);

                        entity.Emails.AddRange(context.Pessoa_Email.Where(a => a.IdPessoa == entity.IdPaciente).ToList());
                        entity.Telefones.AddRange(context.Pessoa_Telefone.Where(a => a.IdPessoa == entity.IdPaciente).ToList());
                    }
                }

                return entity;
            }
        }

        public List<PacienteListResult> GetList(PacienteFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                List<PacienteListResult> listEntity = null;

                if (filterView == null || filterView.IsEmpty)
                {
                    listEntity = (from paciente in context.Paciente
                                  join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                                  join clinica in context.Clinica on paciente.IdClinica equals clinica.IdClinica
                                  where paciente.IdClinica == IdClinicaLogada
                                  select new PacienteListResult
                                  {
                                      Status = paciente.Status,
                                      IdPaciente = paciente.IdPaciente,
                                      Nome = pessoa.Nome,
                                      CPF = pessoa.CPF,
                                      Codigo = paciente.Codigo,
                                      SiglaClinica = clinica.Sigla
                                  }
                                  )
                                  .OrderByDescending(a => a.IdPaciente)
                                  .Take(100)
                                  .ToList();
                }
                else
                {
                    listEntity = (from paciente in context.Paciente
                                  join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                                  join clinica in context.Clinica on paciente.IdClinica equals clinica.IdClinica
                                  where paciente.IdClinica == IdClinicaLogada
                                     && (filterView.Nome.Trim() == null || pessoa.Nome.Contains(filterView.Nome))
                                     && (filterView.CPF.Trim() == null || pessoa.CPF.Equals(filterView.CPF.Replace(".", "").Replace("-", "")))
                                     && (filterView.RG.Trim() == null || pessoa.RG.Equals(filterView.RG.Replace(".", "").Replace("-", "")))
                                     && (filterView.ConsiderarInativos || (filterView.ConsiderarInativos == false && paciente.Status == (int)DefaultStatusEnum.Ativo))
                                     && (filterView.IdPacienteStatus.HasValue == false || paciente.IdPacienteStatus == filterView.IdPacienteStatus.Value)
                                  select new PacienteListResult
                                  {
                                      Status = paciente.Status,
                                      IdPaciente = paciente.IdPaciente,
                                      Nome = pessoa.Nome,
                                      CPF = pessoa.CPF,
                                      Codigo = paciente.Codigo,
                                      SiglaClinica = clinica.Sigla
                                  }
                                  )
                                  .OrderByDescending(a => a.IdPaciente)
                                  .ToList();
                }

                return listEntity;
            }
        }

        public bool Save(PacienteForm entity)
        {
            if (entity.IdPaciente == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(PacienteForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entityPessoa = new Pessoa();
                entityPessoa.IdUsuarioCadastro = IdUsuarioLogado;
                entityPessoa.IdUsuarioAlteracao = IdUsuarioLogado;
                entityPessoa.DataHoraCadastro = CurrentDateTime;
                entityPessoa.DataHoraAlteracao = CurrentDateTime;
                entityPessoa.Status = (int)DefaultStatusEnum.Ativo;
                entityPessoa.Nacionalidade = model.Nacionalidade;
                entityPessoa.Nome = model.Nome;
                entityPessoa.Sexo = model.Sexo;
                entityPessoa.DataNascimento = model.DataNascimento;
                entityPessoa.IdProfissao = model.IdProfissao;
                entityPessoa.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entityPessoa.NumeroEndereco = model.NumeroEndereco;
                entityPessoa.Complemento = model.Complemento;
                entityPessoa.CPF = model.CPF;
                entityPessoa.RG = model.RG;
                entityPessoa.EstadoCivil = model.EstadoCivil;

                model.Telefones.ForEach(item =>
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdPessoa = entityPessoa.IdPessoa;

                    context.Pessoa_Telefone.Add(item);
                });

                model.Emails.ForEach(item =>
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdPessoa = entityPessoa.IdPessoa;

                    context.Pessoa_Email.Add(item);
                });

                var entity = new Paciente();
                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.IdClinica = IdClinicaLogada;
                entity.Codigo = GetProximoCodigo();
                entity.IdPacienteStatus = context.PacienteStatus.First(a => a.Inicial).IdPacienteStatus;
                entity.IdInstitutoRadiologia = model.IdInstitutoRadiologia;
                entity.IdMarketing = model.IdMarketing;
                entity.IdPacienteIndicacao = model.IdPacienteIndicacao;
                entity.TipoIndicacao = model.TipoIndicacao;

                entity.DiaVencimento = model.DiaVencimento;
                entity.Observacao = model.Observacao;
                entity.DataAtivacao = model.DataAtivacao;

                // Por padrão, quando o cliente está sendo criado, o aditivo / contrato não é discutido.
                entity.AditivoImpresso = false;
                entity.ExibirMensagemAditivo = model.ExibirMensagemAditivo;
                entity.ExibirMensagemAgenda = model.ExibirMensagemAgenda;
                entity.DeclaranteIR = model.DeclaranteIR;

                entity.IdPessoa = entityPessoa.IdPessoa;

                context.Set<Paciente>().Add(entity);

                RegistrarLogAtividade(context, entity, PERMISSAO_CADASTRO);
                return context.SaveChanges() > 0;
            }
        }

        private bool Update(PacienteForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdPaciente);
                if (entity == null) throw new BusinessException("Cliente não encontrado");

                var entityPessoa = context.Pessoa.First(a => a.IdPessoa == entity.IdPessoa);

                context.Entry(entityPessoa).State = EntityState.Modified;
                context.Entry(entity).State = EntityState.Modified;

                var listaTelefones = context.Pessoa_Telefone.Where(a => a.IdPessoa == entity.IdPessoa).ToList();
                var listaEmails = context.Pessoa_Email.Where(a => a.IdPessoa == entity.IdPessoa).ToList();
                context.Pessoa_Telefone.RemoveRange(listaTelefones);
                context.Pessoa_Email.RemoveRange(listaEmails);
                
                entityPessoa.IdUsuarioAlteracao = IdUsuarioLogado;
                entityPessoa.DataHoraAlteracao = CurrentDateTime;

                entityPessoa.Status = model.Status;
                entityPessoa.Nacionalidade = model.Nacionalidade;
                entityPessoa.Nome = model.Nome;
                entityPessoa.Sexo = model.Sexo;
                entityPessoa.DataNascimento = model.DataNascimento;
                entityPessoa.IdProfissao = model.IdProfissao;
                entityPessoa.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entityPessoa.NumeroEndereco = model.NumeroEndereco;
                entityPessoa.Complemento = model.Complemento;
                entityPessoa.CPF = model.CPF;
                entityPessoa.RG = model.RG;
                entityPessoa.EstadoCivil = model.EstadoCivil;

                model.Telefones.ForEach(item =>
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdPessoa = entityPessoa.IdPessoa;

                    context.Pessoa_Telefone.Add(item);
                });

                model.Emails.ForEach(item =>
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdPessoa = entityPessoa.IdPessoa;

                    context.Pessoa_Email.Add(item);
                });
                
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = model.Status;
                entity.IdPacienteStatus = context.PacienteStatus.First(a => a.Inicial).IdPacienteStatus;
                entity.IdInstitutoRadiologia = model.IdInstitutoRadiologia;
                entity.IdMarketing = model.IdMarketing;
                entity.IdPacienteIndicacao = model.IdPacienteIndicacao;
                entity.TipoIndicacao = model.TipoIndicacao;

                entity.DiaVencimento = model.DiaVencimento;
                entity.Observacao = model.Observacao;
                entity.DataAtivacao = model.DataAtivacao;

                // Por padrão, quando o cliente está sendo criado, o aditivo / contrato não é discutido.
                entity.AditivoImpresso = model.AditivoImpresso;
                entity.ExibirMensagemAditivo = model.ExibirMensagemAditivo;
                entity.ExibirMensagemAgenda = model.ExibirMensagemAgenda;
                entity.DeclaranteIR = model.DeclaranteIR;

                RegistrarLogAtividade(context, entity, PERMISSAO_CADASTRO);
                return context.SaveChanges() > 0;
            }
        }
        #endregion

        /// <summary>
        /// Retorna a estatística de pacientes que não contém algumas informações cadastradas consideradas mínima, como CPF, data de nascimento dentre outros.
        /// </summary>
        /// <returns></returns>
        public List<GetPacientesDesatualizadosResult> GetPacientesDesatualizados()
        {
            using (var context = new DatabaseContext())
            {
                var sqlQuery = "SP_PacientesDesatualizados";

                return context.Database.SqlQuery<GetPacientesDesatualizadosResult>(sqlQuery).ToList();
            }
        }

        /// <summary>
        /// Retorna a lista de aniversariantes do dia
        /// </summary>
        /// <returns>Lista de pacientes que fazem aniversário no dia</returns>
        public List<GetAniversariantesDiaResult> GetAniversariantesDia()
        {
            using (var context = new DatabaseContext())
            {
                return (from paciente in context.Paciente
                        join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                        where paciente.Status == (int)DefaultStatusEnum.Ativo
                           && pessoa.DataNascimento.Value.Day == CurrentDateTime.Day
                           && pessoa.DataNascimento.Value.Month == CurrentDateTime.Month
                        select new GetAniversariantesDiaResult
                        {
                            Nome = pessoa.Nome
                        })
                        .OrderBy(a => a.Nome)
                        .ToList();
            }
        }

        /// <summary>
        /// Pesquisa destinada unicamente ao componente de auto complete. Esta pesquisa tem algumas particularidades.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<PacienteAutoCompleteResult> AutoComplete(string text)
        {
            using (var context = new DatabaseContext())
            {
                var retorno = new List<PacienteAutoCompleteResult>();

                int codigo = 0;
                if (text != null && int.TryParse(text.Substring(3), out codigo))
                {
                    retorno = (from paciente in context.Paciente
                               join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                               where paciente.IdClinica == IdClinicaLogada && paciente.Codigo == codigo
                               select new PacienteAutoCompleteResult
                               {
                                   IdPaciente = paciente.IdPaciente,
                                   Nome = pessoa.Nome
                               })
                            .Take(ROWS_LIMIT_AUTOCOMPLETE)
                            .ToList();
                }
                else
                {
                    retorno = (from paciente in context.Paciente
                               join pessoa in context.Pessoa on paciente.IdPessoa equals pessoa.IdPessoa
                               where paciente.IdClinica == IdClinicaLogada && pessoa.Nome.Contains(text) && paciente.Status == (int)DefaultStatusEnum.Ativo
                               select new PacienteAutoCompleteResult
                               {
                                   IdPaciente = paciente.IdPaciente,
                                   Nome = pessoa.Nome
                               })
                        .Take(ROWS_LIMIT_AUTOCOMPLETE)
                        .ToList();
                }

                return retorno;
            }
        }

        /// <summary>
        /// Registra uma satisfação do cliente
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="currentContext"></param>
        /// <returns></returns>
        public bool RegistrarSatisfacao(Satisfacao entity, DatabaseContext currentContext = null)
        {
            var idClinica = GetLoggedUser().IdClinica;

            entity.DataHoraCadastro = DateTime.Now;
            entity.DataHoraAlteracao = DateTime.Now;
            entity.Status = (int)DefaultStatusEnum.Ativo;
            entity.IdUsuarioCadastro = GetLoggedUser().IdUsuario;
            entity.IdUsuarioAlteracao = GetLoggedUser().IdUsuario;

            if (currentContext == null)
            {
                using (var context = new DatabaseContext())
                {
                    context.Satisfacao.Add(entity);
                    var result = context.SaveChanges() > 0;
                    if (result) RegistrarLogAtividade(entity, PERMISSAO_GERENCIAR_SATISFACAO);
                    return result;
                }
            }
            else
            {
                currentContext.Satisfacao.Add(entity);
            }

            return true;
        }

        /// <summary>
        /// Método utilizado no agendamento, quando é com um novo paciente, então o cadastro é bem simples e rápido.
        /// </summary>
        /// <returns></returns>
        public int SalvarParaAgendamento(DatabaseContext context, PacienteAgendamentoForm model)
        {
            var entityPessoa = new Pessoa();
            entityPessoa.IdUsuarioCadastro = IdUsuarioLogado;
            entityPessoa.IdUsuarioAlteracao = IdUsuarioLogado;
            entityPessoa.DataHoraCadastro = CurrentDateTime;
            entityPessoa.DataHoraAlteracao = CurrentDateTime;
            entityPessoa.Status = (int)DefaultStatusEnum.Ativo;
            entityPessoa.Nacionalidade = "Brasileira";
            entityPessoa.Nome = model.Nome;
            entityPessoa.Sexo = model.Sexo;
            entityPessoa.DataNascimento = model.DataNascimento;

            model.Telefones.ForEach(item =>
            {
                item.DataHoraCadastro = CurrentDateTime;
                item.DataHoraAlteracao = CurrentDateTime;
                item.IdUsuarioCadastro = IdUsuarioLogado;
                item.IdUsuarioAlteracao = IdUsuarioLogado;
                item.Status = (int)DefaultStatusEnum.Ativo;
                item.IdPessoa = entityPessoa.IdPessoa;

                context.Pessoa_Telefone.Add(item);
            });

            model.Emails.ForEach(item =>
            {
                item.DataHoraCadastro = CurrentDateTime;
                item.DataHoraAlteracao = CurrentDateTime;
                item.IdUsuarioCadastro = IdUsuarioLogado;
                item.IdUsuarioAlteracao = IdUsuarioLogado;
                item.Status = (int)DefaultStatusEnum.Ativo;
                item.IdPessoa = entityPessoa.IdPessoa;

                context.Pessoa_Email.Add(item);
            });

            var entity = new Paciente();
            entity.DataHoraCadastro = CurrentDateTime;
            entity.DataHoraAlteracao = CurrentDateTime;
            entity.IdUsuarioCadastro = IdUsuarioLogado;
            entity.IdUsuarioAlteracao = IdUsuarioLogado;
            entity.Status = (int)DefaultStatusEnum.Ativo;
            entity.IdClinica = IdClinicaLogada;
            entity.Codigo = GetProximoCodigo();
            entity.IdPacienteIndicacao = model.IdPacienteIndicacao;
            entity.IdPessoa = entityPessoa.IdPessoa;

            context.Set<Pessoa>().Add(entityPessoa);
            context.Set<Paciente>().Add(entity);

            // O save context é performado pela Agenda.
            // context.SaveChanges();
            return entity.IdPaciente;
        }

        /// <summary>
        /// Retorna o próximo código do cliente para a clínica.
        /// </summary>
        /// <param name="idClinica"></param>
        /// <returns></returns>
        private int GetProximoCodigo()
        {
            using (var context = new DatabaseContext())
            {
                return context.Paciente.Max(a => a.Codigo.Value) + 1;
            }
        }
    }
}
