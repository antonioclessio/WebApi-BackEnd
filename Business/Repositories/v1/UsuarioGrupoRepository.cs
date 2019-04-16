using System.Linq;
using System.Collections.Generic;
using Business.Interfaces.V1;
using Entities.Entity.Table;
using Entities.Entity.UsuarioGrupo;
using Entities.Exceptions;
using Entities.Enum;
using System.Data.Entity;
using System;

namespace Business.Repositories.v1
{
    public class UsuarioGrupoRepository : BaseRepository, IRepository<UsuarioGrupo, UsuarioGrupoListResult, UsuarioGrupoDetailResult, UsuarioGrupoFilterQuery, UsuarioGrupoForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Usuario;

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(key);
                context.Entry(entity).State = EntityState.Deleted;

                var aplicacoesSelecionadas = context.UsuarioGrupo_Permissao.Where(a => a.IdUsuarioGrupo == key).ToList();
                foreach (var item in aplicacoesSelecionadas) context.UsuarioGrupo_Permissao.Remove(item);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_EXCLUSAO);
                return result;
            }
        }

        public UsuarioGrupoDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.UsuarioGrupo.FirstOrDefault(a => a.IdUsuarioGrupo == key);
                if (entity == null) return null;

                return new UsuarioGrupoDetailResult
                {
                    IdUsuarioGrupo = entity.IdUsuarioGrupo,
                    Status = entity.Status,
                    Nome = entity.Nome
                };
            }
        }

        public UsuarioGrupo GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.UsuarioGrupo.FirstOrDefault(a => a.IdUsuarioGrupo == key);
            }
        }

        public UsuarioGrupoForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.UsuarioGrupo.FirstOrDefault(a => a.IdUsuarioGrupo == key);
                if (entity == null) return null;

                var aplicacoesSelecionadas = context.UsuarioGrupo_Permissao.Where(a => a.IdUsuarioGrupo == key).ToList();
                var aplicacoesParse = new List<Aplicacao_Permissao>();

                foreach (var item in aplicacoesSelecionadas)
                {
                    var indices = Array.ConvertAll(item.Permissoes.Split('.'), s => int.Parse(s));
                    foreach (var indice in indices)
                    {
                        aplicacoesParse.Add(context.Aplicacao_Permissao.First(a => a.IdAplicacao == item.IdAplicacao && a.Indice == indice));
                    }
                }

                return new UsuarioGrupoForm
                {
                    IdUsuarioGrupo = entity.IdUsuarioGrupo,
                    Status = entity.Status,
                    Nome = entity.Nome,
                    TipoPerfil = entity.TipoPerfil,
                    Permissoes = aplicacoesParse
                };
            }
        }

        public List<UsuarioGrupoListResult> GetList(UsuarioGrupoFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                if (filterView == null || filterView.IsEmpty)
                    return (from grupo in context.UsuarioGrupo
                            where grupo.Status == (int)DefaultStatusEnum.Ativo
                            select new UsuarioGrupoListResult
                            {
                                IdUsuarioGrupo = grupo.IdUsuarioGrupo,
                                Nome = grupo.Nome,
                                Status = grupo.Status
                            })
                            .Take(ROWS_LIMIT)
                            .ToList();

                return (from grupo in context.UsuarioGrupo
                        where (filterView.Nome.Trim() == null || grupo.Nome.Contains(filterView.Nome))
                           && (filterView.Status.HasValue == false || grupo.Status == filterView.Status.Value)
                        select new UsuarioGrupoListResult
                        {
                            IdUsuarioGrupo = grupo.IdUsuarioGrupo,
                            Nome = grupo.Nome,
                            Status = grupo.Status
                        })
                        .ToList();
            }
        }

        public bool Save(UsuarioGrupoForm entity)
        {
            if (entity.IdUsuarioGrupo == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(UsuarioGrupoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new UsuarioGrupo();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.TipoPerfil = model.TipoPerfil;

                context.Set<UsuarioGrupo>().Add(entity);

                var permissoes = ParsePermissoes(model.Permissoes);
                foreach (var item in permissoes)
                {
                    item.IdUsuarioGrupo = model.IdUsuarioGrupo;
                    context.Set<UsuarioGrupo_Permissao>().Add(item);
                }

                var result = context.SaveChanges() > 0;

                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(UsuarioGrupoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdUsuarioGrupo);
                if (entity == null) return false;

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Nome = model.Nome;
                entity.TipoPerfil = model.TipoPerfil;

                var aplicacoesSelecionadas = context.UsuarioGrupo_Permissao.Where(a => a.IdUsuarioGrupo == model.IdUsuarioGrupo).ToList();
                foreach (var item in aplicacoesSelecionadas) context.UsuarioGrupo_Permissao.Remove(item);

                var permissoes = ParsePermissoes(model.Permissoes);
                foreach (var item in permissoes)
                {
                    item.IdUsuarioGrupo = model.IdUsuarioGrupo;
                    context.Set<UsuarioGrupo_Permissao>().Add(item);
                }

                var result = context.SaveChanges() > 0;

                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }

        private List<UsuarioGrupo_Permissao> ParsePermissoes(List<Aplicacao_Permissao> permissoes)
        {
            var result = new List<UsuarioGrupo_Permissao>();

            foreach (var item in permissoes)
            {
                var appExistente = result.FirstOrDefault(a => a.IdAplicacao == item.IdAplicacao);
                if (appExistente == null)
                {
                    result.Add(new UsuarioGrupo_Permissao {
                        DataHoraCadastro = CurrentDateTime,
                        DataHoraAlteracao = CurrentDateTime,
                        IdUsuarioCadastro = IdUsuarioLogado,
                        IdUsuarioAlteracao = IdUsuarioLogado,
                        Status = (int)DefaultStatusEnum.Ativo,
                        IdAplicacao = item.IdAplicacao,
                        Permissoes = item.Indice.ToString()
                    });
                }
                else
                {
                    appExistente.Permissoes += "." + item.Indice;
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Retorna o grupo de usuário que um determinado usuário pertence em relação à clinica autenticada
        /// </summary>
        /// <param name="idUsuario">Usuário a ser verificado</param>
        /// <param name="idClinica">Clínica a ser verificada</param>
        /// <returns>Grupo encontrado</returns>
        public RetornarPorUsuarioClinicaResult GetByUsuarioClinica(int idUsuario, int idClinica)
        {
            using (var context = new DatabaseContext())
            {
                if (idClinica == 0)
                    throw new BusinessException("Informe a clínica para continuar.");

                var entity = (from grupo in context.UsuarioGrupo
                              join clinica in context.Clinica_Usuario on grupo.IdUsuarioGrupo equals clinica.IdUsuarioGrupo
                              where clinica.IdUsuario == idUsuario && clinica.IdClinica == idClinica
                              select grupo).FirstOrDefault();

                var permissoes = context.UsuarioGrupo_Permissao.Where(a => a.IdUsuarioGrupo == entity.IdUsuarioGrupo).ToList();

                return new RetornarPorUsuarioClinicaResult
                {
                    Grupo = entity,
                    Permissoes = permissoes
                };
            }
        }
    }
}
