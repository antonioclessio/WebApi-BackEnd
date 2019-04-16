using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Especialidade;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class EspecialidadeRepository : BaseRepository, IRepository<Especialidade, EspecialidadeListResult, EspecialidadeDetailResult, EspecialidadeFilterQuery, EspecialidadeForm>
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
                return result;
            }
        }

        public EspecialidadeDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from especialidade in context.Especialidade
                        where especialidade.IdEspecialidade == key
                        select new EspecialidadeDetailResult
                        {
                            IdEspecialidade = especialidade.IdEspecialidade,
                            Nome = especialidade.Nome,
                            Status = especialidade.Status,
                            Cor = especialidade.Cor
                        }).FirstOrDefault();
            }
        }

        public Especialidade GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Especialidade.FirstOrDefault(a => a.IdEspecialidade == key);
            }
        }

        public EspecialidadeForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from especialidade in context.Especialidade
                        where especialidade.IdEspecialidade == key
                        select new EspecialidadeForm
                        {
                            IdEspecialidade = especialidade.IdEspecialidade,
                            Nome = especialidade.Nome,
                            Status = especialidade.Status,
                            Cor = especialidade.Cor
                        }).FirstOrDefault();
            }
        }

        public List<EspecialidadeListResult> GetList(EspecialidadeFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<EspecialidadeListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from especialidade in context.Especialidade
                              where especialidade.Status == (int)DefaultStatusEnum.Ativo
                              select new EspecialidadeListResult
                              {
                                  IdEspecialidade = especialidade.IdEspecialidade,
                                  Status = especialidade.Status,
                                  Nome = especialidade.Nome,
                                  Cor = especialidade.Cor
                              })
                              .Take(ROWS_LIMIT)
                              .ToList();
                }
                else
                {
                    result = (from especialidade in context.Especialidade
                              where (filterView.Status.HasValue == false || especialidade.Status == filterView.Status.Value)
                                 && (string.IsNullOrEmpty(filterView.Nome) == false || especialidade.Nome.Contains(filterView.Nome))
                              select new EspecialidadeListResult
                              {
                                  IdEspecialidade = especialidade.IdEspecialidade,
                                  Status = especialidade.Status,
                                  Nome = especialidade.Nome,
                                  Cor = especialidade.Cor
                              })
                              .ToList();
                }

                return result;
            }
        }

        public bool Save(EspecialidadeForm entity)
        {
            if (entity.IdEspecialidade == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(EspecialidadeForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Especialidade();
                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.Cor = model.Cor;

                context.Set<Especialidade>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(EspecialidadeForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdEspecialidade);
                if (entity == null) throw new BusinessException("Especialidade não encontrada");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = model.Status;
                entity.Nome = model.Nome;
                entity.Cor = model.Cor;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion
    }
}
