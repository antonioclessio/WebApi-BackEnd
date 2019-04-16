using GrupoOrto.ERP.Entities.Entity.Profissao;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;
using GrupoOrto.ERP.Business.Interfaces;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class ProfissaoRepository : BaseRepository, IProfissaoRepository
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Neutro;

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(key);
                context.Entry(entity).State = EntityState.Deleted;

                RegistrarLogAtividade(context, entity, PERMISSAO_EXCLUSAO);
                return context.SaveChanges() > 0;
            }
        }

        public ProfissaoDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from profissao in context.Profissao
                        where profissao.IdProfissao == key
                        select new ProfissaoDetailResult
                        {
                            IdProfissao = profissao.IdProfissao,
                            Status = profissao.Status,
                            Nome = profissao.Nome
                        })
                        .FirstOrDefault();
            }
        }

        public Profissao GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Profissao.FirstOrDefault(a => a.IdProfissao == key);
            }
        }

        public ProfissaoForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from profissao in context.Profissao
                        where profissao.IdProfissao == key
                        select new ProfissaoForm
                        {
                            IdProfissao = profissao.IdProfissao,
                            Status = profissao.Status,
                            Nome = profissao.Nome
                        })
                        .FirstOrDefault();
            }
        }

        public List<ProfissaoListResult> GetList(ProfissaoFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<ProfissaoListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from profissao in context.Profissao
                              where profissao.Status == (int)DefaultStatusEnum.Ativo
                              select new ProfissaoListResult
                              {
                                  IdProfissao = profissao.IdProfissao,
                                  Status = profissao.Status,
                                  Nome = profissao.Nome
                              })
                              .Take(ROWS_LIMIT)
                              .ToList();
                }
                else
                {
                    result = (from profissao in context.Profissao
                              where profissao.Nome.Contains(filterView.Nome)
                              select new ProfissaoListResult
                              {
                                  IdProfissao = profissao.IdProfissao,
                                  Status = profissao.Status,
                                  Nome = profissao.Nome
                              })
                              .ToList();
                }

                return result;
            }
        }

        public bool Save(ProfissaoForm entity)
        {
            if (entity.IdProfissao == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(ProfissaoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Profissao();
                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;

                context.Set<Profissao>().Add(entity);

                RegistrarLogAtividade(context, entity, PERMISSAO_CADASTRO);
                return context.SaveChanges() > 0;
            }
        }

        private bool Update(ProfissaoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdProfissao);
                if (entity == null) throw new BusinessException("Profissão não encontrada");

                context.Entry(entity).State = EntityState.Modified;
                
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = model.Status;
                entity.Nome = model.Nome;

                RegistrarLogAtividade(context, entity, PERMISSAO_ALTERACAO);
                return context.SaveChanges() > 0;
            }
        }
        #endregion
    }
}
