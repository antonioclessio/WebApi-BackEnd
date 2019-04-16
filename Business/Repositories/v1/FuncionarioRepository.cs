using Business.Interfaces.V1;
using Entities.Entity.Funcionario;
using Entities.Entity.Table;
using Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using Entities.Exceptions;

namespace Business.Repositories.v1
{
    public class FuncionarioRepository : BaseRepository, IRepository<Funcionario, FuncionarioListResult, FuncionarioDetailResult, FuncionarioFilterQuery, FuncionarioForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Neutro;

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(key);
                context.Entry(entity).State = EntityState.Deleted;

                var listaEmails = context.Funcionario_Email.Where(a => a.IdFuncionario == key).ToList();
                var listaTelefones = context.Funcionario_Telefone.Where(a => a.IdFuncionario == key).ToList();

                context.Funcionario_Email.RemoveRange(listaEmails);
                context.Funcionario_Telefone.RemoveRange(listaTelefones);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_EXCLUSAO);
                return context.SaveChanges() > 0;
            }
        }

        public FuncionarioDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from funcionario in context.Funcionario
                        where funcionario.Status == (int)DefaultStatusEnum.Ativo
                           && funcionario.IdFuncionario == key
                        select new FuncionarioDetailResult
                        {
                            IdFuncionario = funcionario.IdFuncionario,
                            Nome = funcionario.Nome,
                            CPF = funcionario.CPF,
                            Status = funcionario.Status,
                            CarteiraProfissionalNro = funcionario.CarteiraProfissionalNro,
                            CarteiraProfissionalSerie = funcionario.CarteiraProfissionalSerie,
                            Complemento = funcionario.Complemento,
                            DataAdmissao = funcionario.DataAdmissao,
                            DataDesligamento = funcionario.DataDesligamento,
                            DataNascimento = funcionario.DataNascimento,
                            EstadoCivil = funcionario.EstadoCivil,
                            IdClinica = funcionario.IdClinica,
                            IdLocalizacaoGeografica = funcionario.IdLocalizacaoGeografica,
                            IdUsuario = funcionario.IdUsuario,
                            NumeroEndereco = funcionario.NumeroEndereco,
                            Observacao = funcionario.Observacao,
                            RG = funcionario.RG
                        }).FirstOrDefault();
            }
        }

        public Funcionario GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Funcionario.FirstOrDefault(a => a.IdFuncionario == key);
            }
        }

        public FuncionarioForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from funcionario in context.Funcionario
                              where funcionario.Status == (int)DefaultStatusEnum.Ativo
                                 && funcionario.IdFuncionario == key
                              select new FuncionarioForm
                              {
                                  IdFuncionario = funcionario.IdFuncionario,
                                  Nome = funcionario.Nome,
                                  CPF = funcionario.CPF,
                                  Status = funcionario.Status,
                                  CarteiraProfissionalNro = funcionario.CarteiraProfissionalNro,
                                  CarteiraProfissionalSerie = funcionario.CarteiraProfissionalSerie,
                                  Complemento = funcionario.Complemento,
                                  DataAdmissao = funcionario.DataAdmissao,
                                  DataDesligamento = funcionario.DataDesligamento,
                                  DataNascimento = funcionario.DataNascimento,
                                  EstadoCivil = funcionario.EstadoCivil,
                                  IdClinica = funcionario.IdClinica,
                                  IdLocalizacaoGeografica = funcionario.IdLocalizacaoGeografica,
                                  IdUsuario = funcionario.IdUsuario,
                                  NumeroEndereco = funcionario.NumeroEndereco,
                                  Observacao = funcionario.Observacao,
                                  RG = funcionario.RG
                              }).FirstOrDefault();

                if (entity != null)
                {
                    if (entity.IdLocalizacaoGeografica.HasValue)
                    {
                        var geoLogRep = new LocalizacaoGeograficaRepository();
                        entity.LocalizacaoGeografica = geoLogRep.GetByKey(entity.IdLocalizacaoGeografica.Value);
                    }

                    entity.Emails.AddRange((from email in context.Funcionario_Email
                                            where email.IdFuncionario == entity.IdFuncionario
                                            select new FuncionarioEmailDetail
                                            {
                                                DataHoraAlteracao = email.DataHoraAlteracao,
                                                IdFuncionario = email.IdFuncionario,
                                                DataHoraCadastro = email.DataHoraCadastro,
                                                Email = email.Email,
                                                IdFuncionarioEmail = email.IdFuncionarioEmail,
                                                IdUsuarioAlteracao = email.IdUsuarioAlteracao,
                                                IdUsuarioCadastro = email.IdUsuarioCadastro,
                                                Status = email.Status,
                                                Tipo = email.Tipo
                                            }).ToList());

                    entity.Telefones.AddRange((from telefone in context.Funcionario_Telefone
                                               where telefone.IdFuncionario == entity.IdFuncionario
                                               select new FuncionarioTelefoneDetail
                                               {
                                                   DataHoraAlteracao = telefone.DataHoraAlteracao,
                                                   IdFuncionario = telefone.IdFuncionario,
                                                   DataHoraCadastro = telefone.DataHoraCadastro,
                                                   Numero = telefone.Numero,
                                                   IdFuncionarioTelefone = telefone.IdFuncionarioTelefone,
                                                   IdUsuarioAlteracao = telefone.IdUsuarioAlteracao,
                                                   IdUsuarioCadastro = telefone.IdUsuarioCadastro,
                                                   Status = telefone.Status,
                                                   Tipo = telefone.Tipo
                                               }).ToList());
                }

                return entity;
            }
        }

        public List<FuncionarioListResult> GetList(FuncionarioFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<FuncionarioListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from funcionario in context.Funcionario
                              where funcionario.Status == (int)DefaultStatusEnum.Ativo
                                 && funcionario.IdClinica == IdClinicaLogada
                              select new FuncionarioListResult
                              {
                                  IdFuncionario = funcionario.IdFuncionario,
                                  Nome = funcionario.Nome,
                                  Status = funcionario.Status,
                                  CPF = funcionario.CPF
                              })
                              .OrderByDescending(a => a.IdFuncionario)
                              .Take(ROWS_LIMIT)
                              .ToList();
                }
                else
                {
                    result = (from funcionario in context.Funcionario
                              where funcionario.IdClinica == IdClinicaLogada
                                 && (filterView.Nome.Trim() == null || funcionario.Nome.Contains(filterView.Nome))
                                 && (filterView.CPF.Trim() == null || funcionario.CPF.Equals(filterView.CPF.Replace(".", "").Replace("-", "")))
                                 && (filterView.Status.HasValue == false || funcionario.Status == filterView.Status.Value)
                                 && (filterView.DataAdmissao.HasValue == false || funcionario.DataAdmissao == filterView.DataAdmissao.Value)
                                 && (filterView.DataDesligamento.HasValue == false || funcionario.DataDesligamento == filterView.DataDesligamento.Value)
                              select new FuncionarioListResult
                              {
                                  IdFuncionario = funcionario.IdFuncionario,
                                  Nome = funcionario.Nome,
                                  Status = funcionario.Status,
                                  CPF = funcionario.CPF
                              }).ToList();
                }

                return result;
            }
        }

        public bool Save(FuncionarioForm entity)
        {
            if (entity.IdFuncionario == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(FuncionarioForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Funcionario();
                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                entity.IdUsuario = model.IdUsuario;
                entity.IdClinica = model.IdClinica;
                entity.Nome = model.Nome;
                entity.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entity.NumeroEndereco = model.NumeroEndereco;
                entity.EstadoCivil = model.EstadoCivil;
                entity.CPF = model.CPF;
                entity.RG = model.RG;
                entity.DataNascimento = model.DataNascimento;
                entity.DataAdmissao = model.DataAdmissao;
                entity.DataDesligamento = model.DataDesligamento;
                entity.CarteiraProfissionalNro = model.CarteiraProfissionalNro;
                entity.CarteiraProfissionalSerie = model.CarteiraProfissionalSerie;
                entity.Observacao = model.Observacao;
                entity.Complemento = model.Complemento;

                model.Telefones.ForEach(item =>
                {
                    var telefoneEntity = new Funcionario_Telefone();
                    telefoneEntity.DataHoraCadastro = CurrentDateTime;
                    telefoneEntity.DataHoraAlteracao = CurrentDateTime;
                    telefoneEntity.IdUsuarioCadastro = IdUsuarioLogado;
                    telefoneEntity.IdUsuarioAlteracao = IdUsuarioLogado;
                    telefoneEntity.Status = (int)DefaultStatusEnum.Ativo;
                    telefoneEntity.IdFuncionario = entity.IdFuncionario;
                    telefoneEntity.Tipo = item.Tipo;
                    telefoneEntity.Numero = item.Numero;

                    context.Set<Funcionario_Telefone>().Add(telefoneEntity);
                });

                model.Emails.ForEach(item =>
                {
                    var emailEntity = new Funcionario_Email();
                    emailEntity.DataHoraCadastro = CurrentDateTime;
                    emailEntity.DataHoraAlteracao = CurrentDateTime;
                    emailEntity.IdUsuarioCadastro = IdUsuarioLogado;
                    emailEntity.IdUsuarioAlteracao = IdUsuarioLogado;
                    emailEntity.Status = (int)DefaultStatusEnum.Ativo;
                    emailEntity.IdFuncionario = entity.IdFuncionario;
                    emailEntity.Tipo = item.Tipo;
                    emailEntity.Email = item.Email;

                    context.Set<Funcionario_Email>().Add(emailEntity);
                });

                context.Set<Funcionario>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(FuncionarioForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdFuncionario);
                if (entity == null) throw new BusinessException("Colaborador não encontrado");

                context.Entry(entity).State = EntityState.Modified;

                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.Status = model.Status;

                entity.IdUsuario = model.IdUsuario;
                entity.IdClinica = model.IdClinica;
                entity.Nome = model.Nome;
                entity.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entity.NumeroEndereco = model.NumeroEndereco;
                entity.EstadoCivil = model.EstadoCivil;
                entity.CPF = model.CPF;
                entity.RG = model.RG;
                entity.DataNascimento = model.DataNascimento;
                entity.DataAdmissao = model.DataAdmissao;
                entity.DataDesligamento = model.DataDesligamento;
                entity.CarteiraProfissionalNro = model.CarteiraProfissionalNro;
                entity.CarteiraProfissionalSerie = model.CarteiraProfissionalSerie;
                entity.Observacao = model.Observacao;
                entity.Complemento = model.Complemento;

                var listTelefone = context.Funcionario_Telefone.Where(a => a.IdFuncionario == entity.IdFuncionario).ToList();
                var listEmail = context.Funcionario_Email.Where(a => a.IdFuncionario == entity.IdFuncionario).ToList();

                context.Funcionario_Telefone.RemoveRange(listTelefone);
                context.Funcionario_Email.RemoveRange(listEmail);

                model.Telefones.ForEach(item =>
                {
                    var telefoneEntity = new Funcionario_Telefone();
                    telefoneEntity.DataHoraCadastro = CurrentDateTime;
                    telefoneEntity.DataHoraAlteracao = CurrentDateTime;
                    telefoneEntity.IdUsuarioCadastro = IdUsuarioLogado;
                    telefoneEntity.IdUsuarioAlteracao = IdUsuarioLogado;
                    telefoneEntity.Status = (int)DefaultStatusEnum.Ativo;
                    telefoneEntity.IdFuncionario = entity.IdFuncionario;
                    telefoneEntity.Tipo = item.Tipo;
                    telefoneEntity.Numero = item.Numero;

                    context.Set<Funcionario_Telefone>().Add(telefoneEntity);
                });

                model.Emails.ForEach(item =>
                {
                    var emailEntity = new Funcionario_Email();
                    emailEntity.DataHoraCadastro = CurrentDateTime;
                    emailEntity.DataHoraAlteracao = CurrentDateTime;
                    emailEntity.IdUsuarioCadastro = IdUsuarioLogado;
                    emailEntity.IdUsuarioAlteracao = IdUsuarioLogado;
                    emailEntity.Status = (int)DefaultStatusEnum.Ativo;
                    emailEntity.IdFuncionario = entity.IdFuncionario;
                    emailEntity.Tipo = item.Tipo;
                    emailEntity.Email = item.Email;

                    context.Set<Funcionario_Email>().Add(emailEntity);
                });

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion
    }
}
