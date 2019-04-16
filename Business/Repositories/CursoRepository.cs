using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Curso;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class CursoRepository : BaseRepository, IRepository<Curso, CursoListResult, CursoDetailResult, CursoFilterQuery, CursoForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Curso;

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

        public CursoDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from curso in context.Curso
                        join doutor in context.Doutor on curso.IdDoutor equals doutor.IdDoutor
                        where curso.IdCurso == key
                        select new CursoDetailResult
                        {
                            IdCurso = curso.IdCurso,
                            Status = curso.Status,
                            NomeCurso = curso.Nome,
                            NomeDoutor = doutor.Nome,
                            Data = curso.DataCurso
                        }).FirstOrDefault();
            }
        }

        public Curso GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Curso.FirstOrDefault(a => a.IdCurso == key);
            }
        }

        public CursoForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from curso in context.Curso
                        join doutor in context.Doutor on curso.IdDoutor equals doutor.IdDoutor
                        where curso.IdCurso == key
                        select new CursoForm
                        {
                            IdCurso = curso.IdCurso,
                            Status = curso.Status,
                            IdDoutor = doutor.IdDoutor,
                            Data = curso.DataCurso,
                            Nome = curso.Nome
                        }).FirstOrDefault();
            }
        }

        public List<CursoListResult> GetList(CursoFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<CursoListResult>();
                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from curso in context.Curso
                            join doutor in context.Doutor on curso.IdDoutor equals doutor.IdDoutor
                            where curso.Status == (int)DefaultStatusEnum.Ativo
                              select new CursoListResult
                            {
                                IdCurso = curso.IdCurso,
                                Status = curso.Status,
                                NomeCurso = curso.Nome,
                                NomeDoutor = doutor.Nome,
                                Data = curso.DataCurso
                            })
                            .Take(ROWS_LIMIT)
                            .ToList();
                } else
                {
                    result = (from curso in context.Curso
                              join doutor in context.Doutor on curso.IdDoutor equals doutor.IdDoutor
                              where (filterView.Status.HasValue == false || curso.Status == filterView.Status.Value)
                                 && (string.IsNullOrEmpty(filterView.Nome) == false || curso.Nome == filterView.Nome)
                                 && (filterView.IdDoutor.HasValue == false || curso.IdDoutor == filterView.IdDoutor.Value)
                                 && (filterView.Data.HasValue == false || curso.DataCurso == filterView.Data.Value)
                              select new CursoListResult
                              {
                                  IdCurso = curso.IdCurso,
                                  Status = curso.Status,
                                  NomeCurso = curso.Nome,
                                  NomeDoutor = doutor.Nome,
                                  Data = curso.DataCurso
                              }).ToList();
                }

                return result;
            }
        }

        public bool Save(CursoForm entity)
        {
            if (entity.IdCurso == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(CursoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Curso();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.IdDoutor = model.IdDoutor;
                entity.DataCurso = model.Data;
                entity.Nome = model.Nome;

                context.Set<Curso>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);

                return result;
            }
        }

        private bool Update(CursoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdCurso);

                context.Entry(entity).State = EntityState.Modified;

                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdDoutor = model.IdDoutor;
                entity.DataCurso = model.Data;
                entity.Nome = model.Nome;
                entity.Status = model.Status;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);

                return result;
            }
        }
        #endregion
    }
}
