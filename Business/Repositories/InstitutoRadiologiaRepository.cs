using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.InstitutoRadiologia;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Entities.Exceptions;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class InstitutoRadiologiaRepository : BaseRepository, IRepository<InstitutoRadiologia, InstitutoRadiologiaListResult, InstitutoRadiologiaDetailResult, InstitutoRadiologiaFilterQuery, InstitutoRadiologiaForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.InstitutoRadiologia;

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

        public InstitutoRadiologiaDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var result = (from institutoRadiologia in context.InstitutoRadiologia
                              where institutoRadiologia.IdInstitutoRadiologia == key
                              select new InstitutoRadiologiaDetailResult
                              {
                                  IdInstitutoRadiologia = institutoRadiologia.IdInstitutoRadiologia,
                                  Complemento = institutoRadiologia.Complemento,
                                  HomePage = institutoRadiologia.HomePage,
                                  Nome = institutoRadiologia.Nome,
                                  IdLocalizacaoGeografica = institutoRadiologia.IdLocalizacaoGeografica,
                                  NumeroEndereco = institutoRadiologia.NumeroEndereco,
                                  Status = institutoRadiologia.Status
                              })
                        .FirstOrDefault();

                if (result != null)
                {
                    if (result.IdLocalizacaoGeografica.HasValue)
                    {
                        var geoLocRepo = new LocalizacaoGeograficaRepository();
                        result.LocalizacaoGeografica = geoLocRepo.GetByKey(result.IdLocalizacaoGeografica.Value);
                    }

                    result.InstitutoRadiologia_Telefone = new List<InstitutoRadiologia_Telefone>();
                    result.InstitutoRadiologia_Telefone.AddRange(context.InstitutoRadiologia_Telefone.Where(a => a.IdInstitutoRadiologia == result.IdInstitutoRadiologia).ToList());

                    result.InstitutoRadiologia_Email = new List<InstitutoRadiologia_Email>();
                    result.InstitutoRadiologia_Email.AddRange(context.InstitutoRadiologia_Email.Where(a => a.IdInstitutoRadiologia == result.IdInstitutoRadiologia).ToList());
                }

                return result;
            }
        }

        public InstitutoRadiologia GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.InstitutoRadiologia.FirstOrDefault(a => a.IdInstitutoRadiologia == key);
            }
        }

        public InstitutoRadiologiaForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var result = (from institutoRadiologia in context.InstitutoRadiologia
                              where institutoRadiologia.IdInstitutoRadiologia == key
                              select new InstitutoRadiologiaForm
                              {
                                  IdInstitutoRadiologia = institutoRadiologia.IdInstitutoRadiologia,
                                  Complemento = institutoRadiologia.Complemento,
                                  HomePage = institutoRadiologia.HomePage,
                                  Nome = institutoRadiologia.Nome,
                                  IdLocalizacaoGeografica = institutoRadiologia.IdLocalizacaoGeografica,
                                  NumeroEndereco = institutoRadiologia.NumeroEndereco,
                                  Status = institutoRadiologia.Status
                              }).FirstOrDefault();

                if (result != null)
                {
                    result.InstitutoRadiologia_Telefone = new List<InstitutoRadiologia_Telefone>();
                    result.InstitutoRadiologia_Telefone.AddRange(context.InstitutoRadiologia_Telefone.Where(a => a.IdInstitutoRadiologia == result.IdInstitutoRadiologia).ToList());

                    result.InstitutoRadiologia_Email = new List<InstitutoRadiologia_Email>();
                    result.InstitutoRadiologia_Email.AddRange(context.InstitutoRadiologia_Email.Where(a => a.IdInstitutoRadiologia == result.IdInstitutoRadiologia).ToList());
                }

                return result;
            }
        }

        public List<InstitutoRadiologiaListResult> GetList(InstitutoRadiologiaFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                var result = new List<InstitutoRadiologiaListResult>();

                if (filterView == null || filterView.IsEmpty)
                {
                    result = (from institutoRadiologia in context.InstitutoRadiologia
                              where institutoRadiologia.Status == (int)DefaultStatusEnum.Ativo
                              select new InstitutoRadiologiaListResult
                              {
                                  IdInstitutoRadiologia = institutoRadiologia.IdInstitutoRadiologia,
                                  HomePage = institutoRadiologia.HomePage,
                                  Nome = institutoRadiologia.Nome,
                                  Status = institutoRadiologia.Status
                              })
                              .Take(ROWS_LIMIT)
                              .ToList();
                }
                else
                {
                    result = (from institutoRadiologia in context.InstitutoRadiologia
                              where (filterView.Status.HasValue == false || institutoRadiologia.Status == filterView.Status.Value)
                                 && (string.IsNullOrEmpty(filterView.Nome) == false || institutoRadiologia.Nome.Contains(filterView.Nome))
                                 && (string.IsNullOrEmpty(filterView.HomePage) == false || institutoRadiologia.Nome.Contains(filterView.HomePage))
                              select new InstitutoRadiologiaListResult
                              {
                                  IdInstitutoRadiologia = institutoRadiologia.IdInstitutoRadiologia,
                                  HomePage = institutoRadiologia.HomePage,
                                  Nome = institutoRadiologia.Nome,
                                  Status = institutoRadiologia.Status
                              }).ToList();
                }

                return result;
            }
        }

        public bool Save(InstitutoRadiologiaForm entity)
        {
            if (entity.IdInstitutoRadiologia == 0)
                return Insert(entity);

            return Update(entity);
        }

        private bool Insert(InstitutoRadiologiaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new InstitutoRadiologia();
                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                entity.Nome = model.Nome;
                entity.HomePage = model.HomePage;
                entity.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entity.NumeroEndereco = model.NumeroEndereco;
                entity.Complemento = model.Complemento;

                model.InstitutoRadiologia_Telefone.ForEach(item =>
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdInstitutoRadiologia = entity.IdInstitutoRadiologia;

                    context.InstitutoRadiologia_Telefone.Add(item);
                });
                
                model.InstitutoRadiologia_Email.ForEach(item =>
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdInstitutoRadiologia = entity.IdInstitutoRadiologia;

                    context.InstitutoRadiologia_Email.Add(item);
                });

                context.Set<InstitutoRadiologia>().Add(entity);

                RegistrarLogAtividade(context, entity, PERMISSAO_CADASTRO);
                return context.SaveChanges() > 0;
            }
        }

        private bool Update(InstitutoRadiologiaForm model)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(model.IdInstitutoRadiologia);
                if (entity == null) throw new BusinessException("Instituto de radiologia não encontrado");

                context.Entry(entity).State = EntityState.Modified;

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;

                entity.Status = model.Status;
                entity.Nome = model.Nome;
                entity.HomePage = model.HomePage;
                entity.IdLocalizacaoGeografica = model.IdLocalizacaoGeografica;
                entity.NumeroEndereco = model.NumeroEndereco;
                entity.Complemento = model.Complemento;

                var listTelefone = context.InstitutoRadiologia_Telefone.Where(a => a.IdInstitutoRadiologia == entity.IdInstitutoRadiologia);
                var listEmail = context.InstitutoRadiologia_Email.Where(a => a.IdInstitutoRadiologia == entity.IdInstitutoRadiologia);

                context.InstitutoRadiologia_Telefone.RemoveRange(listTelefone);
                context.InstitutoRadiologia_Email.RemoveRange(listEmail);

                model.InstitutoRadiologia_Telefone.ForEach(item =>
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdInstitutoRadiologia = entity.IdInstitutoRadiologia;

                    context.InstitutoRadiologia_Telefone.Add(item);
                });
                
                model.InstitutoRadiologia_Email.ForEach(item =>
                {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdInstitutoRadiologia = entity.IdInstitutoRadiologia;

                    context.InstitutoRadiologia_Email.Add(item);
                });

                RegistrarLogAtividade(context, entity, PERMISSAO_ALTERACAO);
                return context.SaveChanges() > 0;
            }
        }
        #endregion
    }
}
