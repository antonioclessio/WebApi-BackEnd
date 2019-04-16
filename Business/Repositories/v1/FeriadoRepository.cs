using Business.Interfaces.V1;
using Entities.Entity.Feriado;
using Entities.Entity.Table;
using Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using Entities.Exceptions;

namespace Business.Repositories.v1
{
    public class FeriadoRepository : BaseRepository, IRepository<Feriado, FeriadoListResult, FeriadoDetailResult, FeriadoFilterQuery, FeriadoForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Feriados;

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

        public FeriadoDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var result = (from feriado in context.Feriado
                              join cidadeJoin in context.Cidade on feriado.IdCidade equals cidadeJoin.IdCidade into cidades
                              from cidade in cidades.DefaultIfEmpty()
                              join estadoJoin in context.Estado on feriado.IdEstado equals estadoJoin.IdEstado into estados
                              from estado in estados.DefaultIfEmpty()
                              where feriado.IdFeriado == key
                              select new FeriadoDetailResult
                              {
                                  IdFeriado = feriado.IdFeriado,
                                  Data = feriado.Data,
                                  Cidade = cidade.Nome,
                                  Estado = estado.Sigla,
                                  Nome = feriado.Nome,
                                  Status = feriado.Status,
                                  Tipo = feriado.Tipo
                              }).FirstOrDefault();

                return result;
            }
        }

        public Feriado GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Feriado.FirstOrDefault(a => a.IdFeriado == key);
            }
        }

        public FeriadoForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from feriado in context.Feriado
                              where feriado.IdFeriado == key
                              select new FeriadoForm
                              {
                                  IdFeriado = feriado.IdFeriado,
                                  Data = feriado.Data,
                                  IdCidade = feriado.IdCidade,
                                  IdEstado = feriado.IdEstado,
                                  Nome = feriado.Nome,
                                  Status = feriado.Status,
                                  Tipo = feriado.Tipo
                              }).FirstOrDefault();

                if (entity != null && entity.IdCidade.HasValue)
                {
                    entity.NomeCidade = context.Cidade.First(a => a.IdCidade == entity.IdCidade).Nome;
                }

                return entity;
            }
        }

        public List<FeriadoListResult> GetList(FeriadoFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<FeriadoListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from feriado in context.Feriado
                              join cidadeJoin in context.Cidade on feriado.IdCidade equals cidadeJoin.IdCidade into cidades
                              from cidade in cidades.DefaultIfEmpty()
                              join estadoJoin in context.Estado on feriado.IdEstado equals estadoJoin.IdEstado into estados
                              from estado in estados.DefaultIfEmpty()
                              where feriado.Status == (int)DefaultStatusEnum.Ativo
                              select new FeriadoListResult
                              {
                                  IdFeriado = feriado.IdFeriado,
                                  Data = feriado.Data,
                                  Cidade = cidade.Nome,
                                  Estado = estado.Sigla,
                                  Nome = feriado.Nome,
                                  Status = feriado.Status,
                                  Tipo = feriado.Tipo
                              }).Take(ROWS_LIMIT)
                              .ToList();
                }
                else
                {
                    result = (from feriado in context.Feriado
                              join cidadeJoin in context.Cidade on feriado.IdCidade equals cidadeJoin.IdCidade into cidades
                              from cidade in cidades.DefaultIfEmpty()
                              join estadoJoin in context.Estado on feriado.IdEstado equals estadoJoin.IdEstado into estados
                              from estado in estados.DefaultIfEmpty()
                              where (filterView.Status.HasValue == false || feriado.Status == filterView.Status.Value)
                                 && (string.IsNullOrEmpty(filterView.Nome) == true || feriado.Nome == filterView.Nome)
                                 && (filterView.IdEstado.HasValue == false || feriado.IdEstado == filterView.IdEstado.Value)
                                 && (filterView.IdCidade.HasValue == false || feriado.IdCidade == filterView.IdCidade.Value)
                                 && (filterView.Data.HasValue == false || feriado.Data == filterView.Data.Value)
                                 && (filterView.Tipo.HasValue == false || feriado.Tipo == filterView.Tipo.Value)
                              select new FeriadoListResult
                              {
                                  IdFeriado = feriado.IdFeriado,
                                  Data = feriado.Data,
                                  Cidade = cidade.Nome,
                                  Estado = estado.Sigla,
                                  Nome = feriado.Nome,
                                  Status = feriado.Status,
                                  Tipo = feriado.Tipo
                              })
                              .ToList();
                }

                return result;
            }
        }

        public bool Save(FeriadoForm entity)
        {
            if (entity.IdFeriado == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(FeriadoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Feriado();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.Data = model.Data;
                entity.IdCidade = model.IdCidade;
                entity.IdEstado = model.IdEstado;
                entity.Tipo = model.Tipo;

                context.Set<Feriado>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(FeriadoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdFeriado);
                if (entity == null) throw new BusinessException("Feriado não encontrado para alteração");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.Data = model.Data;
                entity.IdCidade = model.IdCidade;
                entity.IdEstado = model.IdEstado;
                entity.Tipo = model.Tipo;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion
    }
}
