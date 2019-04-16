using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Confirmacao;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class ConfirmacaoRepository : BaseRepository, IRepository<Confirmacao, ConfirmacaoListResult, ConfirmacaoDetailResult, ConfirmacaoFilterQuery, ConfirmacaoForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Confirmacao;

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

        public ConfirmacaoDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from confirmacao in context.Confirmacao
                        where confirmacao.IdConfirmacao == key
                        select new ConfirmacaoDetailResult
                        {
                            IdConfirmacao = confirmacao.IdConfirmacao,
                            Nome = confirmacao.Nome,
                            Sigla = confirmacao.Sigla,
                            Status = confirmacao.Status
                        }).FirstOrDefault();
            }
        }

        public Confirmacao GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Confirmacao.FirstOrDefault(a => a.IdConfirmacao == key);
            }
        }

        public ConfirmacaoForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from confirmacao in context.Confirmacao
                        where confirmacao.IdConfirmacao == key
                        select new ConfirmacaoForm
                        {
                            IdConfirmacao = confirmacao.IdConfirmacao,
                            Nome = confirmacao.Nome,
                            Sigla = confirmacao.Sigla,
                            Status = confirmacao.Status
                        }).FirstOrDefault();
            }
        }

        public List<ConfirmacaoListResult> GetList(ConfirmacaoFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<ConfirmacaoListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from confirmacao in context.Confirmacao
                            where confirmacao.Status == (int)DefaultStatusEnum.Ativo
                            select new ConfirmacaoListResult
                            {
                                IdConfirmacao = confirmacao.IdConfirmacao,
                                Nome = confirmacao.Nome,
                                Sigla = confirmacao.Sigla,
                                Status = confirmacao.Status
                            }).Take(ROWS_LIMIT)
                            .ToList();
                } else
                {
                    result = (from confirmacao in context.Confirmacao
                              where (filterView.Status.HasValue == false || confirmacao.Status == filterView.Status.Value)
                                 && (string.IsNullOrEmpty(filterView.Nome) == false || confirmacao.Nome == filterView.Nome)
                                 && (string.IsNullOrEmpty(filterView.Sigla) == false || confirmacao.Sigla == filterView.Sigla)
                              select new ConfirmacaoListResult
                              {
                                  IdConfirmacao = confirmacao.IdConfirmacao,
                                  Nome = confirmacao.Nome,
                                  Sigla = confirmacao.Sigla,
                                  Status = confirmacao.Status
                              })
                            .ToList();
                }

                return result;
            }
        }

        public bool Save(ConfirmacaoForm entity)
        {
            if (entity.IdConfirmacao == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(ConfirmacaoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new Confirmacao();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.Sigla = model.Sigla;

                context.Set<Confirmacao>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }

        private bool Update(ConfirmacaoForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdConfirmacao);
                if (entity == null) throw new BusinessException("Confirmação não encontrada");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.Sigla = model.Sigla;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);
                return result;
            }
        }
        #endregion
    }
}
