using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Funcionario;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;

namespace GrupoOrto.ERP.Business.Repositories
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
                return (from funcionario in context.Funcionario
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
                } else
                {
                    result = (from funcionario in context.Funcionario
                              where (filterView.Nome.Trim() == null || funcionario.Nome.Contains(filterView.Nome))
                                 && (filterView.CPF.Trim() == null || funcionario.CPF.Equals(filterView.CPF.Replace(".", "").Replace("-", "")))
                                 && (filterView.Status.HasValue == false || funcionario.Status == filterView.Status.Value)
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

                model.Funcionario_Telefone.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdFuncionario = entity.IdFuncionario;

                    context.Funcionario_Telefone.Add(item);
                });

                model.Funcionario_Email.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdFuncionario = entity.IdFuncionario;

                    context.Funcionario_Email.Add(item);
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

                model.Funcionario_Telefone.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdFuncionario = entity.IdFuncionario;

                    context.Funcionario_Telefone.Add(item);
                });

                model.Funcionario_Email.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdFuncionario = entity.IdFuncionario;

                    context.Funcionario_Email.Add(item);
                });

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion
    }
}
