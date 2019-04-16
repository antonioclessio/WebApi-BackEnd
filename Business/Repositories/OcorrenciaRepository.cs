using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Ocorrencia;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class OcorrenciaRepository : BaseRepository, IRepository<Ocorrencia, OcorrenciaListResult, OcorrenciaDetailResult, OcorrenciaFilterQuery, OcorrenciaForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Ocorrencia;

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

        public OcorrenciaDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from ocorrencia in context.Ocorrencia
                        where ocorrencia.Status == (int)DefaultStatusEnum.Ativo
                        select new OcorrenciaDetailResult
                        {
                            IdOcorrencia = ocorrencia.IdOcorrencia,
                            Status = ocorrencia.Status,
                            Nome = ocorrencia.Nome,
                            Cor = ocorrencia.Cor,
                            TipoOcorrencia = ocorrencia.TipoOcorrencia
                        })
                        .FirstOrDefault();
            }
        }

        public Ocorrencia GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Ocorrencia.FirstOrDefault(a => a.IdOcorrencia == key);
            }
        }

        public OcorrenciaForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from ocorrencia in context.Ocorrencia
                        where ocorrencia.Status == (int)DefaultStatusEnum.Ativo
                        select new OcorrenciaForm
                        {
                            IdOcorrencia = ocorrencia.IdOcorrencia,
                            Status = ocorrencia.Status,
                            Nome = ocorrencia.Nome,
                            Cor = ocorrencia.Cor,
                            TipoOcorrencia = ocorrencia.TipoOcorrencia
                        })
                        .FirstOrDefault();
            }
        }

        public List<OcorrenciaListResult> GetList(OcorrenciaFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<OcorrenciaListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from ocorrencia in context.Ocorrencia
                              where ocorrencia.Status == (int)DefaultStatusEnum.Ativo
                              select new OcorrenciaListResult
                              {
                                  IdOcorrencia = ocorrencia.IdOcorrencia,
                                  Status = ocorrencia.Status,
                                  Nome = ocorrencia.Nome,
                                  Cor = ocorrencia.Cor,
                                  TipoOcorrencia = ocorrencia.TipoOcorrencia
                              })
                              .OrderBy(a => a.Nome)
                              .ToList();
                } else
                {
                    result = (from ocorrencia in context.Ocorrencia
                              where (filterView.Status.HasValue == false || ocorrencia.Status == filterView.Status)
                                 && (string.IsNullOrEmpty(filterView.Nome) || ocorrencia.Nome == filterView.Nome)
                              select new OcorrenciaListResult
                              {
                                  IdOcorrencia = ocorrencia.IdOcorrencia,
                                  Status = ocorrencia.Status,
                                  Nome = ocorrencia.Nome,
                                  Cor = ocorrencia.Cor,
                                  TipoOcorrencia = ocorrencia.TipoOcorrencia
                              })
                              .OrderBy(a => a.Nome)
                              .ToList();
                }

                return result;
            }
        }

        public bool Save(OcorrenciaForm entity)
        {
            if (entity.IdOcorrencia == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(OcorrenciaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Ocorrencia();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                entity.Nome = model.Nome;
                entity.Cor = model.Cor;
                entity.TipoOcorrencia = model.TipoOcorrencia;

                context.Set<Ocorrencia>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(OcorrenciaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdOcorrencia);
                context.Entry(entity).State = EntityState.Modified;

                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.Nome = model.Nome;
                entity.Cor = model.Cor;
                entity.TipoOcorrencia = model.TipoOcorrencia;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion
    }
}
