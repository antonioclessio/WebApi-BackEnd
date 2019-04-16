using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Doutor;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class DoutorRepository : BaseRepository, IRepository<Doutor, DoutorListResult, DoutorDetailResult, DoutorFilterQuery, DoutorForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Doutor;
        readonly int INTERVALO_AGENDAMENTO = 15;
        readonly short PERMISSAO_AUSENCIA;

        public DoutorRepository()
        {
            PERMISSAO_AUSENCIA = 5;
        }

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Doutor.FirstOrDefault(a => a.IdDoutor == key);
                if (entity == null) throw new BusinessException("Doutor não encontrado");

                var listTelefone = context.Doutor_Telefone.Where(a => a.IdDoutor == entity.IdDoutor).ToList();
                var listEmail = context.Doutor_Email.Where(a => a.IdDoutor == entity.IdDoutor).ToList();
                var listEspecialidade = context.Doutor_Especialidade.Where(a => a.IdDoutor == entity.IdDoutor).ToList();
                var listClinicas = context.Doutor_Clinica.Where(a => a.IdDoutor == entity.IdDoutor).ToList();

                foreach (var item in listTelefone) context.Entry(item).State = EntityState.Deleted;
                foreach (var item in listEmail) context.Entry(item).State = EntityState.Deleted;
                foreach (var item in listEspecialidade) context.Entry(item).State = EntityState.Deleted;
                foreach (var item in listClinicas) context.Entry(item).State = EntityState.Deleted;

                context.Entry(entity).State = EntityState.Deleted;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_EXCLUSAO);
                return result;
            }
        }

        public DoutorDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from doutor in context.Doutor
                              where doutor.IdDoutor == key
                              select new DoutorDetailResult
                              {
                                  IdDoutor = doutor.IdDoutor,
                                  Status = doutor.Status,
                                  Nome = doutor.Nome,
                                  Sigla = doutor.Sigla,
                                  DataHoraCadastro = doutor.DataHoraCadastro,
                                  CPF = doutor.CPF,
                                  CRO = doutor.CRO,
                                  HomePage = doutor.HomePage,
                                  NumeroEndereco = doutor.NumeroEndereco,
                                  Complemento = doutor.Complemento,
                                  IdLocalizacaoGeografica = doutor.IdLocalizacaoGeografica,

                                  Domingo_Manha_Inicio = doutor.Domingo_Manha_Inicio,
                                  Domingo_Manha_Fim = doutor.Domingo_Manha_Fim,
                                  Segunda_Manha_Inicio = doutor.Segunda_Manha_Inicio,
                                  Segunda_Manha_Fim = doutor.Segunda_Manha_Fim,
                                  Terca_Manha_Inicio = doutor.Terca_Manha_Inicio,
                                  Terca_Manha_Fim = doutor.Terca_Manha_Fim,
                                  Quarta_Manha_Inicio = doutor.Quarta_Manha_Inicio,
                                  Quarta_Manha_Fim = doutor.Quarta_Manha_Fim,
                                  Quinta_Manha_Inicio = doutor.Quinta_Manha_Inicio,
                                  Quinta_Manha_Fim = doutor.Quinta_Manha_Fim,
                                  Sexta_Manha_Inicio = doutor.Sexta_Manha_Inicio,
                                  Sexta_Manha_Fim = doutor.Sexta_Manha_Fim,
                                  Sabado_Manha_Inicio = doutor.Sabado_Manha_Inicio,
                                  Sabado_Manha_Fim = doutor.Sabado_Manha_Fim
                              })
                              .FirstOrDefault();

                if (entity != null)
                {
                    if (entity.IdLocalizacaoGeografica != null)
                    {
                        var geoLocRep = new LocalizacaoGeograficaRepository();
                        entity.LocalizacaoGeografica = geoLocRep.GetByKey(entity.IdLocalizacaoGeografica.Value);
                    }

                    entity.Doutor_Email.AddRange(context.Doutor_Email.Where(a => a.IdDoutor == entity.IdDoutor).ToList());
                    entity.Doutor_Telefone.AddRange(context.Doutor_Telefone.Where(a => a.IdDoutor == entity.IdDoutor).ToList());
                }

                return entity;
            }
        }

        public Doutor GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Doutor.FirstOrDefault(a => a.IdDoutor == key);
            }
        }

        public DoutorForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from doutor in context.Doutor
                              where doutor.IdDoutor == key
                              select new DoutorForm
                              {
                                  IdDoutor = doutor.IdDoutor,
                                  Status = doutor.Status,
                                  Nome = doutor.Nome,
                                  CRO = doutor.CRO,
                                  Sigla = doutor.Sigla,
                                  EstadoCivil = doutor.EstadoCivil,
                                  DataNascimento = doutor.DataNascimento,
                                  IdUsuario = doutor.IdUsuario,
                                  Nacionalidade = doutor.Nacionalidade,
                                  PermiteOutroAgendamento = doutor.PermiteAgendamentoOutro,
                                  Capacidade = doutor.Capacidade,
                                  CPF = doutor.CPF,
                                  RG = doutor.RG,
                                  IdLocalizacaoGeografica = doutor.IdLocalizacaoGeografica,
                                  NumeroEndereco = doutor.NumeroEndereco,
                                  Complemento = doutor.Complemento,
                                  DataHoraCadastro = doutor.DataHoraCadastro,
                                  DisponibilidadeSabadosMes = doutor.DisponibilidadeSabadosMes,

                                  Domingo_Manha_Inicio = doutor.Domingo_Manha_Inicio,
                                  Domingo_Manha_Fim = doutor.Domingo_Manha_Fim,
                                  Domingo_Tarde_Inicio = doutor.Domingo_Tarde_Inicio,
                                  Domingo_Tarde_Fim = doutor.Domingo_Tarde_Fim,
                                  Domingo_Noite_Inicio = doutor.Domingo_Noite_Inicio,
                                  Domingo_Noite_Fim = doutor.Domingo_Noite_Fim,

                                  Segunda_Manha_Inicio = doutor.Segunda_Manha_Inicio,
                                  Segunda_Manha_Fim = doutor.Segunda_Manha_Fim,
                                  Segunda_Tarde_Inicio = doutor.Segunda_Tarde_Inicio,
                                  Segunda_Tarde_Fim = doutor.Segunda_Tarde_Fim,
                                  Segunda_Noite_Inicio = doutor.Segunda_Noite_Inicio,
                                  Segunda_Noite_Fim = doutor.Segunda_Noite_Fim,

                                  Terca_Manha_Inicio = doutor.Terca_Manha_Inicio,
                                  Terca_Manha_Fim = doutor.Terca_Manha_Fim,
                                  Terca_Tarde_Inicio = doutor.Terca_Tarde_Inicio,
                                  Terca_Tarde_Fim = doutor.Terca_Tarde_Fim,
                                  Terca_Noite_Inicio = doutor.Terca_Noite_Inicio,
                                  Terca_Noite_Fim = doutor.Terca_Noite_Fim,

                                  Quarta_Manha_Inicio = doutor.Quarta_Manha_Inicio,
                                  Quarta_Manha_Fim = doutor.Quarta_Manha_Fim,
                                  Quarta_Tarde_Inicio = doutor.Quarta_Tarde_Inicio,
                                  Quarta_Tarde_Fim = doutor.Quarta_Tarde_Fim,
                                  Quarta_Noite_Inicio = doutor.Quarta_Noite_Inicio,
                                  Quarta_Noite_Fim = doutor.Quarta_Noite_Fim,

                                  Quinta_Manha_Inicio = doutor.Quinta_Manha_Inicio,
                                  Quinta_Manha_Fim = doutor.Quinta_Manha_Fim,
                                  Quinta_Tarde_Inicio = doutor.Quinta_Tarde_Inicio,
                                  Quinta_Tarde_Fim = doutor.Quinta_Tarde_Fim,
                                  Quinta_Noite_Inicio = doutor.Quinta_Noite_Inicio,
                                  Quinta_Noite_Fim = doutor.Quinta_Noite_Fim,

                                  Sexta_Manha_Inicio = doutor.Sexta_Manha_Inicio,
                                  Sexta_Manha_Fim = doutor.Sexta_Manha_Fim,
                                  Sexta_Tarde_Inicio = doutor.Sexta_Tarde_Inicio,
                                  Sexta_Tarde_Fim = doutor.Sexta_Tarde_Fim,
                                  Sexta_Noite_Inicio = doutor.Sexta_Noite_Inicio,
                                  Sexta_Noite_Fim = doutor.Sexta_Noite_Fim,

                                  Sabado_Manha_Inicio = doutor.Sabado_Manha_Inicio,
                                  Sabado_Manha_Fim = doutor.Sabado_Manha_Fim,
                                  Sabado_Tarde_Inicio = doutor.Sabado_Tarde_Inicio,
                                  Sabado_Tarde_Fim = doutor.Sabado_Tarde_Fim,
                                  Sabado_Noite_Inicio = doutor.Sabado_Noite_Inicio,
                                  Sabado_Noite_Fim = doutor.Sabado_Noite_Fim
                              })
                              .FirstOrDefault();

                if (entity != null)
                {
                    if (entity.IdLocalizacaoGeografica != null)
                    {
                        var geoLocRep = new LocalizacaoGeograficaRepository();
                        entity.LocalizacaoGeografica = geoLocRep.GetByKey(entity.IdLocalizacaoGeografica.Value);
                    }

                    entity.Doutor_Email = new List<Doutor_Email>();
                    entity.Doutor_Telefone = new List<Doutor_Telefone>();
                    entity.Doutor_Clinica = new List<Doutor_Clinica>();
                    entity.Especialidades = new List<Doutor_Especialidade>();

                    entity.Doutor_Email.AddRange(context.Doutor_Email.Where(a => a.IdDoutor == entity.IdDoutor).ToList());
                    entity.Doutor_Telefone.AddRange(context.Doutor_Telefone.Where(a => a.IdDoutor == entity.IdDoutor).ToList());
                    entity.Doutor_Clinica.AddRange(context.Doutor_Clinica.Where(a => a.IdDoutor == entity.IdDoutor).ToList());
                    entity.Especialidades.AddRange(context.Doutor_Especialidade.Where(a => a.IdDoutor == entity.IdDoutor).ToList());
                }

                return entity;
            }
        }

        public List<DoutorListResult> GetList(DoutorFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                List<DoutorListResult> listEntity = null;

                if (filterView == null || filterView.IsEmpty)
                {
                    listEntity = (from doutor in context.Doutor
                                  join clinica in context.Doutor_Clinica on doutor.IdDoutor equals clinica.IdDoutor
                                  where clinica.IdClinica == IdClinicaLogada && doutor.Status == (int)DefaultStatusEnum.Ativo
                                  select new DoutorListResult
                                  {
                                      IdDoutor = doutor.IdDoutor,
                                      Status = doutor.Status,
                                      Nome = doutor.Nome,
                                      CPF = doutor.CPF,
                                      CRO = doutor.CRO,
                                      DataHoraCadastro = doutor.DataHoraCadastro,
                                      Sigla = doutor.Sigla
                                  })
                                  .Distinct()
                                  .OrderByDescending(a => a.DataHoraCadastro)
                                  .ToList();
                }
                else
                {
                    listEntity = (from doutor in context.Doutor
                                  join clinica in context.Doutor_Clinica on doutor.IdDoutor equals clinica.IdDoutor
                                  where clinica.IdClinica == IdClinicaLogada
                                     && (string.IsNullOrEmpty(filterView.Nome) || doutor.Nome.Contains(filterView.Nome))
                                     && (string.IsNullOrEmpty(filterView.CPF) || doutor.CPF.Contains(filterView.CPF))
                                     && (string.IsNullOrEmpty(filterView.CRO) || doutor.CRO.Contains(filterView.CRO))
                                     && (!filterView.Status.HasValue || doutor.Status == filterView.Status.Value)
                                  select new DoutorListResult
                                  {
                                      IdDoutor = doutor.IdDoutor,
                                      Status = doutor.Status,
                                      Nome = doutor.Nome,
                                      CPF = doutor.CPF,
                                      CRO = doutor.CRO,
                                      DataHoraCadastro = doutor.DataHoraCadastro,
                                      Sigla = doutor.Sigla
                                  })
                                  .OrderByDescending(a => a.DataHoraCadastro)
                                  .ToList();
                }

                return listEntity;
            }
        }

        public bool Save(DoutorForm entity)
        {
            var podeGerenciarDisponibilidade = VerificaPermissao(7);

            if (entity.IdDoutor == 0)
                return Insert(entity, podeGerenciarDisponibilidade);

            return Update(entity, podeGerenciarDisponibilidade);
        }

        private bool Insert(DoutorForm model, bool podeGerenciarDisponibilidade)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Doutor();

                entity.DataHoraCadastro = DateTime.Now;
                entity.DataHoraAlteracao = DateTime.Now;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                entity.Nome = model.Nome;
                entity.CRO = model.CRO;
                entity.Sigla = model.Sigla;
                entity.EstadoCivil = model.EstadoCivil;
                entity.DataNascimento = model.DataNascimento;
                entity.IdUsuario = model.IdUsuario;
                entity.Nacionalidade = model.Nacionalidade;
                entity.PermiteAgendamentoOutro = model.PermiteOutroAgendamento;
                entity.Capacidade = model.Capacidade;
                entity.CPF = model.CPF;
                entity.RG = model.RG;
                entity.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entity.NumeroEndereco = model.NumeroEndereco;
                entity.Complemento = model.Complemento;

                entity.QtdKmAjudaCusto = model.QtdKmAjudaCusto;
                entity.KmPorLitro = model.KmPorLitro;

                foreach (var item in model.Doutor_Telefone)
                {
                    context.Set<Doutor_Telefone>().Add(new Doutor_Telefone
                    {
                        DataHoraCadastro = CurrentDateTime,
                        DataHoraAlteracao = CurrentDateTime,
                        IdUsuarioCadastro = IdUsuarioLogado,
                        IdUsuarioAlteracao = IdUsuarioLogado,
                        Status = (int)DefaultStatusEnum.Ativo,
                        IdDoutor = entity.IdDoutor,
                        Tipo = item.Tipo,
                        Numero = item.Numero
                    });
                }

                foreach (var item in model.Doutor_Email)
                {
                    context.Set<Doutor_Email>().Add(new Doutor_Email
                    {
                        DataHoraCadastro = CurrentDateTime,
                        DataHoraAlteracao = CurrentDateTime,
                        IdUsuarioCadastro = IdUsuarioLogado,
                        IdUsuarioAlteracao = IdUsuarioLogado,
                        Status = (int)DefaultStatusEnum.Ativo,
                        IdDoutor = entity.IdDoutor,
                        Tipo = item.Tipo,
                        Email = item.Email
                    });
                }

                foreach (var item in model.Especialidades)
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdDoutor = entity.IdDoutor;

                    context.Set<Doutor_Especialidade>().Add(item);
                }

                if (podeGerenciarDisponibilidade)
                {
                    entity.Domingo_Manha_Inicio = model.Domingo_Manha_Inicio;
                    entity.Domingo_Manha_Fim = model.Domingo_Manha_Fim;
                    entity.Domingo_Tarde_Inicio = model.Domingo_Tarde_Inicio;
                    entity.Domingo_Tarde_Fim = model.Domingo_Tarde_Fim;
                    entity.Domingo_Noite_Inicio = model.Domingo_Noite_Inicio;
                    entity.Domingo_Noite_Fim = model.Domingo_Noite_Fim;

                    entity.Segunda_Manha_Inicio = model.Segunda_Manha_Inicio;
                    entity.Segunda_Manha_Fim = model.Segunda_Manha_Fim;
                    entity.Segunda_Tarde_Inicio = model.Segunda_Tarde_Inicio;
                    entity.Segunda_Tarde_Fim = model.Segunda_Tarde_Fim;
                    entity.Segunda_Noite_Inicio = model.Segunda_Noite_Inicio;
                    entity.Segunda_Noite_Fim = model.Segunda_Noite_Fim;

                    entity.Terca_Manha_Inicio = model.Terca_Manha_Inicio;
                    entity.Terca_Manha_Fim = model.Terca_Manha_Fim;
                    entity.Terca_Tarde_Inicio = model.Terca_Tarde_Inicio;
                    entity.Terca_Tarde_Fim = model.Terca_Tarde_Fim;
                    entity.Terca_Noite_Inicio = model.Terca_Noite_Inicio;
                    entity.Terca_Noite_Fim = model.Terca_Noite_Fim;

                    entity.Quarta_Manha_Inicio = model.Quarta_Manha_Inicio;
                    entity.Quarta_Manha_Fim = model.Quarta_Manha_Fim;
                    entity.Quarta_Tarde_Inicio = model.Quarta_Tarde_Inicio;
                    entity.Quarta_Tarde_Fim = model.Quarta_Tarde_Fim;
                    entity.Quarta_Noite_Inicio = model.Quarta_Noite_Inicio;
                    entity.Quarta_Noite_Fim = model.Quarta_Noite_Fim;

                    entity.Quinta_Manha_Inicio = model.Quinta_Manha_Inicio;
                    entity.Quinta_Manha_Fim = model.Quinta_Manha_Fim;
                    entity.Quinta_Tarde_Inicio = model.Quinta_Tarde_Inicio;
                    entity.Quinta_Tarde_Fim = model.Quinta_Tarde_Fim;
                    entity.Quinta_Noite_Inicio = model.Quinta_Noite_Inicio;
                    entity.Quinta_Noite_Fim = model.Quinta_Noite_Fim;

                    entity.Sexta_Manha_Inicio = model.Sexta_Manha_Inicio;
                    entity.Sexta_Manha_Fim = model.Sexta_Manha_Fim;
                    entity.Sexta_Tarde_Inicio = model.Sexta_Tarde_Inicio;
                    entity.Sexta_Tarde_Fim = model.Sexta_Tarde_Fim;
                    entity.Sexta_Noite_Inicio = model.Sexta_Noite_Inicio;
                    entity.Sexta_Noite_Fim = model.Sexta_Noite_Fim;

                    entity.Sabado_Manha_Inicio = model.Sabado_Manha_Inicio;
                    entity.Sabado_Manha_Fim = model.Sabado_Manha_Fim;
                    entity.Sabado_Tarde_Inicio = model.Sabado_Tarde_Inicio;
                    entity.Sabado_Tarde_Fim = model.Sabado_Tarde_Fim;
                    entity.Sabado_Noite_Inicio = model.Sabado_Noite_Inicio;
                    entity.Sabado_Noite_Fim = model.Sabado_Noite_Fim;

                    entity.DisponibilidadeSabadosMes = model.DisponibilidadeSabadosMes;

                    foreach (var item in model.Doutor_Clinica)
                    {
                        context.Set<Doutor_Clinica>().Add(new Doutor_Clinica
                        {
                            DataHoraCadastro = CurrentDateTime,
                            DataHoraAlteracao = CurrentDateTime,
                            IdUsuarioCadastro = IdUsuarioLogado,
                            IdUsuarioAlteracao = IdUsuarioLogado,
                            Status = (int)DefaultStatusEnum.Ativo,
                            IdDoutor = entity.IdDoutor,
                            IdClinica = item.IdClinica
                        });
                    }
                }

                context.Set<Doutor>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(DoutorForm model, bool podeGerenciarDisponibilidade)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdDoutor);
                if (entity == null) throw new BusinessException("Doutor não encontrado");

                var listTelefone = context.Doutor_Telefone.Where(a => a.IdDoutor == model.IdDoutor).ToList();
                var listEmail = context.Doutor_Email.Where(a => a.IdDoutor == model.IdDoutor).ToList();
                var listEspecialidade = context.Doutor_Especialidade.Where(a => a.IdDoutor == model.IdDoutor).ToList();
                var listaClinica = context.Doutor_Clinica.Where(a => a.IdDoutor == model.IdDoutor).ToList();

                context.Doutor_Telefone.RemoveRange(listTelefone);
                context.Doutor_Email.RemoveRange(listEmail);
                context.Doutor_Especialidade.RemoveRange(listEspecialidade);
                context.Doutor_Clinica.RemoveRange(listaClinica);

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;

                entity.Nome = model.Nome;
                entity.CRO = model.CRO;
                entity.Sigla = model.Sigla;
                entity.EstadoCivil = model.EstadoCivil;
                entity.DataNascimento = model.DataNascimento;
                entity.IdUsuario = model.IdUsuario;
                entity.Nacionalidade = model.Nacionalidade;
                entity.PermiteAgendamentoOutro = model.PermiteOutroAgendamento;
                entity.Capacidade = model.Capacidade;
                entity.CPF = model.CPF;
                entity.RG = model.RG;
                entity.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entity.NumeroEndereco = model.NumeroEndereco;
                entity.Complemento = model.Complemento;

                entity.QtdKmAjudaCusto = model.QtdKmAjudaCusto;
                entity.KmPorLitro = model.KmPorLitro;

                foreach (var item in model.Doutor_Telefone)
                {
                    context.Set<Doutor_Telefone>().Add(new Doutor_Telefone
                    {
                        DataHoraCadastro = CurrentDateTime,
                        DataHoraAlteracao = CurrentDateTime,
                        IdUsuarioCadastro = IdUsuarioLogado,
                        IdUsuarioAlteracao = IdUsuarioLogado,
                        Status = (int)DefaultStatusEnum.Ativo,
                        IdDoutor = entity.IdDoutor,
                        Tipo = item.Tipo,
                        Numero = item.Numero
                    });
                }

                foreach (var item in model.Doutor_Email)
                {
                    context.Set<Doutor_Email>().Add(new Doutor_Email
                    {
                        DataHoraCadastro = CurrentDateTime,
                        DataHoraAlteracao = CurrentDateTime,
                        IdUsuarioCadastro = IdUsuarioLogado,
                        IdUsuarioAlteracao = IdUsuarioLogado,
                        Status = (int)DefaultStatusEnum.Ativo,
                        IdDoutor = entity.IdDoutor,
                        Tipo = item.Tipo,
                        Email = item.Email
                    });
                }

                foreach (var item in model.Especialidades)
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;

                    context.Set<Doutor_Especialidade>().Add(item);
                }

                if (podeGerenciarDisponibilidade)
                {
                    entity.Domingo_Manha_Inicio = model.Domingo_Manha_Inicio;
                    entity.Domingo_Manha_Fim = model.Domingo_Manha_Fim;
                    entity.Domingo_Tarde_Inicio = model.Domingo_Tarde_Inicio;
                    entity.Domingo_Tarde_Fim = model.Domingo_Tarde_Fim;
                    entity.Domingo_Noite_Inicio = model.Domingo_Noite_Inicio;
                    entity.Domingo_Noite_Fim = model.Domingo_Noite_Fim;

                    entity.Segunda_Manha_Inicio = model.Segunda_Manha_Inicio;
                    entity.Segunda_Manha_Fim = model.Segunda_Manha_Fim;
                    entity.Segunda_Tarde_Inicio = model.Segunda_Tarde_Inicio;
                    entity.Segunda_Tarde_Fim = model.Segunda_Tarde_Fim;
                    entity.Segunda_Noite_Inicio = model.Segunda_Noite_Inicio;
                    entity.Segunda_Noite_Fim = model.Segunda_Noite_Fim;

                    entity.Terca_Manha_Inicio = model.Terca_Manha_Inicio;
                    entity.Terca_Manha_Fim = model.Terca_Manha_Fim;
                    entity.Terca_Tarde_Inicio = model.Terca_Tarde_Inicio;
                    entity.Terca_Tarde_Fim = model.Terca_Tarde_Fim;
                    entity.Terca_Noite_Inicio = model.Terca_Noite_Inicio;
                    entity.Terca_Noite_Fim = model.Terca_Noite_Fim;

                    entity.Quarta_Manha_Inicio = model.Quarta_Manha_Inicio;
                    entity.Quarta_Manha_Fim = model.Quarta_Manha_Fim;
                    entity.Quarta_Tarde_Inicio = model.Quarta_Tarde_Inicio;
                    entity.Quarta_Tarde_Fim = model.Quarta_Tarde_Fim;
                    entity.Quarta_Noite_Inicio = model.Quarta_Noite_Inicio;
                    entity.Quarta_Noite_Fim = model.Quarta_Noite_Fim;

                    entity.Quinta_Manha_Inicio = model.Quinta_Manha_Inicio;
                    entity.Quinta_Manha_Fim = model.Quinta_Manha_Fim;
                    entity.Quinta_Tarde_Inicio = model.Quinta_Tarde_Inicio;
                    entity.Quinta_Tarde_Fim = model.Quinta_Tarde_Fim;
                    entity.Quinta_Noite_Inicio = model.Quinta_Noite_Inicio;
                    entity.Quinta_Noite_Fim = model.Quinta_Noite_Fim;

                    entity.Sexta_Manha_Inicio = model.Sexta_Manha_Inicio;
                    entity.Sexta_Manha_Fim = model.Sexta_Manha_Fim;
                    entity.Sexta_Tarde_Inicio = model.Sexta_Tarde_Inicio;
                    entity.Sexta_Tarde_Fim = model.Sexta_Tarde_Fim;
                    entity.Sexta_Noite_Inicio = model.Sexta_Noite_Inicio;
                    entity.Sexta_Noite_Fim = model.Sexta_Noite_Fim;

                    entity.Sabado_Manha_Inicio = model.Sabado_Manha_Inicio;
                    entity.Sabado_Manha_Fim = model.Sabado_Manha_Fim;
                    entity.Sabado_Tarde_Inicio = model.Sabado_Tarde_Inicio;
                    entity.Sabado_Tarde_Fim = model.Sabado_Tarde_Fim;
                    entity.Sabado_Noite_Inicio = model.Sabado_Noite_Inicio;
                    entity.Sabado_Noite_Fim = model.Sabado_Noite_Fim;

                    entity.DisponibilidadeSabadosMes = model.DisponibilidadeSabadosMes;

                    foreach (var item in model.Doutor_Clinica)
                    {
                        context.Set<Doutor_Clinica>().Add(new Doutor_Clinica
                        {
                            DataHoraCadastro = CurrentDateTime,
                            DataHoraAlteracao = CurrentDateTime,
                            IdUsuarioCadastro = IdUsuarioLogado,
                            IdUsuarioAlteracao = IdUsuarioLogado,
                            Status = (int)DefaultStatusEnum.Ativo,
                            IdDoutor = entity.IdDoutor,
                            IdClinica = item.IdClinica
                        });
                    }
                }

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion

        #region >> Public Methods
        public List<DoutorListResult> GetListOrdenada()
        {
            using (var context = new DatabaseContext())
            {
                return (from doutor in context.Doutor
                        join clinica in context.Doutor_Clinica on doutor.IdDoutor equals clinica.IdDoutor
                        where clinica.IdClinica == IdClinicaLogada && doutor.Status == (int)DefaultStatusEnum.Ativo
                        select new DoutorListResult
                        {
                            IdDoutor = doutor.IdDoutor,
                            Status = doutor.Status,
                            Nome = doutor.Nome,
                            CPF = doutor.CPF,
                            CRO = doutor.CRO,
                            DataHoraCadastro = doutor.DataHoraCadastro,
                            Sigla = doutor.Sigla
                        })
                        .OrderBy(a => a.Nome)
                        .ToList();
            }
        }

        public DoutorForm GetByCRO(string cro)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Doutor.FirstOrDefault(a => a.CRO == cro);
                if (entity == null) return null;

                return GetForEdit(entity.IdDoutor);
            }
        }
        #endregion
    }
}
