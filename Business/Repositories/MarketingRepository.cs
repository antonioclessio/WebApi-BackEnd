using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Marketing;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class MarketingRepository : BaseRepository, IRepository<Marketing, MarketingListResult, MarketingDetailResult, MarketingFilterQuery, MarketingForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Marketing;

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(key);
                context.Entry(entity).State = EntityState.Deleted;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_EXCLUSAO);
                return result;
            }
        }

        public MarketingDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from marketing in context.Marketing
                        where marketing.IdMarketing == key
                        select new MarketingDetailResult
                        {
                            IdMarketing = marketing.IdMarketing,
                            Nome = marketing.Nome,
                            Status = marketing.Status,
                            VinculaPaciente = marketing.VinculaPaciente
                        }).FirstOrDefault();
            }
        }

        public Marketing GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Marketing.FirstOrDefault(a => a.IdMarketing == key);
            }
        }

        public MarketingForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from marketing in context.Marketing
                        where marketing.IdMarketing == key
                        select new MarketingForm
                        {
                            IdMarketing = marketing.IdMarketing,
                            Nome = marketing.Nome,
                            Status = marketing.Status,
                            VinculaPaciente = marketing.VinculaPaciente
                        }).FirstOrDefault();
            }
        }

        public List<MarketingListResult> GetList(MarketingFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<MarketingListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from marketing in context.Marketing
                              where marketing.Status == (int)DefaultStatusEnum.Ativo
                              select new MarketingListResult
                              {
                                  IdMarketing = marketing.IdMarketing,
                                  Nome = marketing.Nome,
                                  Status = marketing.Status,
                                  VinculaPaciente = marketing.VinculaPaciente
                              })
                              .Take(ROWS_LIMIT)
                              .ToList();
                } else
                {
                    result = (from marketing in context.Marketing
                              where (filterView.Status.HasValue == false || marketing.Status == filterView.Status.Value)
                                 && (string.IsNullOrEmpty(filterView.Nome) == false || marketing.Nome == filterView.Nome)
                                 && (filterView.VinculaPaciente.HasValue == false || marketing.VinculaPaciente == filterView.VinculaPaciente.Value)
                              select new MarketingListResult
                              {
                                  IdMarketing = marketing.IdMarketing,
                                  Nome = marketing.Nome,
                                  Status = marketing.Status,
                                  VinculaPaciente = marketing.VinculaPaciente
                              }).ToList();
                }

                return result;
            }
        }

        public bool Save(MarketingForm entity)
        {
            if (entity.IdMarketing == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(MarketingForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Marketing();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.VinculaPaciente = model.VinculaPaciente;

                context.Set<Marketing>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(MarketingForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdMarketing);
                if (entity == null) throw new BusinessException("Marketing não encontrado para alteração");

                context.Entry(entity).State = EntityState.Modified;
                
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.VinculaPaciente = model.VinculaPaciente;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion
    }
}
