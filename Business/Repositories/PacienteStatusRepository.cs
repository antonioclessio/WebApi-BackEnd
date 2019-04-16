using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.PacienteStatus;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class PacienteStatusRepository : BaseRepository, IRepository<PacienteStatus, PacienteStatusListResult, PacienteStatusDetailResult, PacienteStatusFilterQuery, PacienteStatusForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.PacienteStatus;

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

        public PacienteStatusDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from pacienteStatus in context.PacienteStatus
                        where pacienteStatus.IdPacienteStatus == key
                        select new PacienteStatusDetailResult
                        {
                            IdPacienteStatus = pacienteStatus.IdPacienteStatus,
                            Status = pacienteStatus.Status,
                            Cor = pacienteStatus.Cor,
                            Inicial = pacienteStatus.Inicial,
                            Nome = pacienteStatus.Nome
                        }).FirstOrDefault();
            }
        }

        public PacienteStatus GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.PacienteStatus.FirstOrDefault(a => a.IdPacienteStatus == key);
            }
        }

        public PacienteStatusForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                return (from pacienteStatus in context.PacienteStatus
                        where pacienteStatus.IdPacienteStatus == key
                        select new PacienteStatusForm
                        {
                            IdPacienteStatus = pacienteStatus.IdPacienteStatus,
                            Status = pacienteStatus.Status,
                            Cor = pacienteStatus.Cor,
                            Inicial = pacienteStatus.Inicial,
                            Nome = pacienteStatus.Nome
                        }).FirstOrDefault();
            }
        }

        public List<PacienteStatusListResult> GetList(PacienteStatusFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<PacienteStatusListResult>();
                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from pacienteStatus in context.PacienteStatus
                              where pacienteStatus.Status == (int)DefaultStatusEnum.Ativo
                              select new PacienteStatusListResult
                              {
                                  IdPacienteStatus = pacienteStatus.IdPacienteStatus,
                                  Status = pacienteStatus.Status,
                                  Cor = pacienteStatus.Cor,
                                  Inicial = pacienteStatus.Inicial,
                                  Nome = pacienteStatus.Nome
                              })
                            .Take(ROWS_LIMIT)
                            .ToList();
                }
                else
                {
                    result = (from pacienteStatus in context.PacienteStatus
                              where (filterView.Status.HasValue == false || pacienteStatus.Status == filterView.Status.Value)
                                 && (string.IsNullOrEmpty(filterView.Nome) == false || pacienteStatus.Nome == filterView.Nome)
                                 && (string.IsNullOrEmpty(filterView.Cor) == false || pacienteStatus.Cor == filterView.Cor)
                              select new PacienteStatusListResult
                              {
                                  IdPacienteStatus = pacienteStatus.IdPacienteStatus,
                                  Status = pacienteStatus.Status,
                                  Cor = pacienteStatus.Cor,
                                  Inicial = pacienteStatus.Inicial,
                                  Nome = pacienteStatus.Nome
                              }).ToList();
                }

                return result;
            }
        }

        public bool Save(PacienteStatusForm entity)
        {
            if (entity.IdPacienteStatus == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(PacienteStatusForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new PacienteStatus();

                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Nome = model.Nome;
                entity.Cor = model.Cor;
                entity.Inicial = model.Inicial;

                context.Set<PacienteStatus>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);

                return result;
            }
        }

        private bool Update(PacienteStatusForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdPacienteStatus);

                context.Entry(entity).State = EntityState.Modified;

                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.Nome = model.Nome;
                entity.Cor = model.Cor;
                entity.Inicial = model.Inicial;

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_ALTERACAO);

                return result;
            }
        }
        #endregion
    }
}
