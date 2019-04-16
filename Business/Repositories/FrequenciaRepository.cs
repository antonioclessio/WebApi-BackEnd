using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Frequencia;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class FrequenciaRepository : BaseRepository, IRepository<Frequencia, FrequenciaListResult, FrequenciaDetailResult, FrequenciaFilterQuery, FrequenciaForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Frequencia;

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

        public FrequenciaDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from frequencia in context.Frequencia
                        where frequencia.Status == (int)DefaultStatusEnum.Ativo
                        select new FrequenciaDetailResult
                        {
                            IdFrequencia = frequencia.IdFrequencia,
                            Status = frequencia.Status,
                            Inicial = frequencia.Inicial,
                            Nome = frequencia.Nome,
                            Cor = frequencia.Cor,
                            Tipo = frequencia.Tipo
                        })
                        .FirstOrDefault();
            }
        }

        public Frequencia GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Frequencia.FirstOrDefault(a => a.IdFrequencia == key);
            }
        }

        public FrequenciaForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from frequencia in context.Frequencia
                        where frequencia.Status == (int)DefaultStatusEnum.Ativo
                        select new FrequenciaForm
                        {
                            IdFrequencia = frequencia.IdFrequencia,
                            Status = frequencia.Status,
                            Inicial = frequencia.Inicial,
                            Nome = frequencia.Nome,
                            Cor = frequencia.Cor,
                            Tipo = frequencia.Tipo
                        })
                        .FirstOrDefault();
            }
        }

        public List<FrequenciaListResult> GetList(FrequenciaFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<FrequenciaListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from frequencia in context.Frequencia
                              where frequencia.Status == (int)DefaultStatusEnum.Ativo
                              select new FrequenciaListResult
                              {
                                  IdFrequencia = frequencia.IdFrequencia,
                                  Status = frequencia.Status,
                                  Inicial = frequencia.Inicial,
                                  Nome = frequencia.Nome,
                                  Cor = frequencia.Cor,
                                  Tipo = frequencia.Tipo
                              })
                              .OrderBy(a => a.Nome)
                              .ToList();
                }
                else
                {
                    result = (from frequencia in context.Frequencia
                              where (filterView.Status.HasValue == false || frequencia.Status == filterView.Status)
                                 && (string.IsNullOrEmpty(filterView.Nome) || frequencia.Nome == filterView.Nome)
                              select new FrequenciaListResult
                              {
                                  IdFrequencia = frequencia.IdFrequencia,
                                  Status = frequencia.Status,
                                  Inicial = frequencia.Inicial,
                                  Nome = frequencia.Nome,
                                  Cor = frequencia.Cor,
                                  Tipo = frequencia.Tipo
                              })
                              .OrderBy(a => a.Nome)
                              .ToList();
                }

                return result;
            }
        }

        public bool Save(FrequenciaForm entity)
        {
            if (entity.IdFrequencia == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(FrequenciaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Frequencia();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                entity.Nome = model.Nome;
                entity.Inicial = model.Inicial;
                entity.Cor = model.Cor;
                entity.Tipo = model.Tipo;

                context.Set<Frequencia>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(FrequenciaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdFrequencia);
                context.Entry(entity).State = EntityState.Modified;

                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.DataHoraAlteracao = CurrentDateTime;

                entity.Nome = model.Nome;
                entity.Inicial = model.Inicial;
                entity.Cor = model.Cor;
                entity.Tipo = model.Tipo;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion
    }
}
