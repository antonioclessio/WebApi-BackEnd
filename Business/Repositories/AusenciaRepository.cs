using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Ausencia;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using GrupoOrto.ERP.Entities.Exceptions;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class AusenciaRepository : BaseRepository, IRepository<Ausencia, AusenciaListResult, AusenciaDetailResult, AusenciaFilterQuery, AusenciaForm>
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Ausencias;
        readonly short PERMISSAO_REGISTRAR_RETORNO;

        public AusenciaRepository()
        {
            PERMISSAO_REGISTRAR_RETORNO = 5;
        }

        #region # Interface
        public bool Delete(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = GetByKeyFull(key);
                context.Entry(entity).State = EntityState.Deleted;

                var listSubstituicoes = context.Ausencia_Substituicao.Where(a => a.IdAusencia == key).ToList();
                context.Ausencia_Substituicao.RemoveRange(listSubstituicoes);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_EXCLUSAO);
                return result;
            }
        }

        public AusenciaDetailResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from ausencia in context.Ausencia
                              where ausencia.IdAusencia == key
                              select new AusenciaDetailResult
                              {
                                  IdAusencia = ausencia.IdAusencia,
                                  DataHoraFim = ausencia.DataHoraFim,
                                  DataHoraInicio = ausencia.DataHoraInicio,
                                  IdDoutor = ausencia.IdDoutor,
                                  Observacao = ausencia.Observacao,
                                  SemJustificativa = ausencia.SemJustificativa,
                                  Status = ausencia.Status,
                                  TipoAusencia = ausencia.TipoAusencia
                              })
                              .FirstOrDefault();

                return entity;
            }
        }

        public Ausencia GetByKeyFull(int key)
        {
            using (var context = new DatabaseContext())
            {
                return context.Ausencia.FirstOrDefault(a => a.IdAusencia == key);
            }
        }

        public AusenciaForm GetForEdit(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = (from ausencia in context.Ausencia
                              where ausencia.IdAusencia == key
                              select new AusenciaForm
                              {
                                  IdAusencia = ausencia.IdAusencia
                              })
                              .FirstOrDefault();

                return entity;
            }
        }

        public List<AusenciaListResult> GetList(AusenciaFilterQuery filterView)
        {
            using (var context = new DatabaseContext())
            {
                if (filterView == null)
                    throw new BusinessException("Informe os parâmetros para filtrar a lista de ausências");

                var listEntity = (from ausencia in context.Ausencia
                                  where (filterView.IdDoutor.HasValue == false || ausencia.IdDoutor == filterView.IdDoutor.Value)
                                     && (filterView.Ano.HasValue == false || ausencia.DataHoraInicio.Value.Year == filterView.Ano.Value)
                                     && (filterView.Mes.HasValue == false || ausencia.DataHoraInicio.Value.Month == filterView.Mes.Value)
                                  select new AusenciaListResult {
                                      IdAusencia = ausencia.IdAusencia,
                                      TipoAusencia = ausencia.TipoAusencia,
                                      DataHoraInicio = ausencia.DataHoraInicio,
                                      DataHoraFim = ausencia.DataHoraFim,
                                      SemJustificativa = ausencia.SemJustificativa,
                                      Observacao = ausencia.Observacao,
                                      ObservacaoRetorno = ausencia.ObservacaoRetorno
                                  }).ToList();

                return listEntity;
            }
        }

        public bool Save(AusenciaForm model)
        {
            using (var context = new DatabaseContext())
            {
                if (model.DataHoraInicio.HasValue && context.Ausencia.FirstOrDefault(a => a.IdDoutor == model.IdDoutor && a.DataHoraInicio.Value == model.DataHoraInicio.Value) != null)
                    throw new BusinessException("O período da ausência coincide com outra já cadastrada. Verifique.");

                if (model.DataHoraInicio.HasValue && model.DataHoraInicio.Value.Date <= DateTime.Now.Date)
                    throw new BusinessException("Não é permitido o cadastro de ausências retroativas.");

                if (model.DataHoraFim.HasValue && model.DataHoraFim.Value < model.DataHoraInicio)
                    throw new BusinessException("A data de retorno não pode ser anterior à data de saída.");

                if (model.TipoAusencia != (int)TipoAusenciaEnum.CancelamentoAgenda)
                {
                    var ultimaAusenciaRegistrada = context.Ausencia.Where(a => a.IdDoutor == model.IdDoutor).OrderByDescending(a => a.IdAusencia).Take(1).FirstOrDefault();
                    if (ultimaAusenciaRegistrada != null && !ultimaAusenciaRegistrada.DataHoraFim.HasValue) throw new BusinessException("Existe uma ausência sem retorno registrado. Verifique.");
                }

                var entity = new Ausencia();
                entity.DataHoraCadastro = CurrentDateTime;
                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioCadastro = IdUsuarioLogado;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.Status = (int)DefaultStatusEnum.Ativo;

                entity.DataHoraInicio = model.DataHoraInicio;
                entity.DataHoraFim = model.DataHoraFim;
                entity.TipoAusencia = model.TipoAusencia;
                entity.SemJustificativa = model.SemJustificativa;
                entity.Observacao = model.Observacao;
                entity.IdDoutor = model.IdDoutor;

                model.Substituicao.ForEach(item => {
                    item.DataHoraCadastro = CurrentDateTime;
                    item.DataHoraAlteracao = CurrentDateTime;
                    item.IdUsuarioCadastro = IdUsuarioLogado;
                    item.IdUsuarioAlteracao = IdUsuarioLogado;
                    item.Status = (int)DefaultStatusEnum.Ativo;
                    item.IdAusencia = entity.IdAusencia;

                    context.Set<Ausencia_Substituicao>().Add(item);
                });

                context.Set<Ausencia>().Add(entity);

                var result = context.SaveChanges() > 0;
                if (result) RegistrarLogAtividade(entity, PERMISSAO_CADASTRO);
                return result;
            }
        }
        #endregion

        /// <summary>
        /// Retorna a lista de doutores que irão compor as substituições de uma ausência
        /// </summary>
        /// <param name="idAusencia"></param>
        /// <returns></returns>
        public List<GetSubstituicoesResult> GetSubstituicoes(int idAusencia)
        {
            using (var context = new DatabaseContext())
            {
                return (from substituicao in context.Ausencia_Substituicao
                        join agenda in context.Agenda on substituicao.IdAgenda equals agenda.IdAgenda
                        join doutorSubstituto in context.Doutor on substituicao.IdDoutorSubstituto equals doutorSubstituto.IdDoutor
                        where substituicao.Status == (int)DefaultStatusEnum.Ativo
                           && substituicao.IdAusencia == idAusencia
                        select new GetSubstituicoesResult
                        {
                            IdAusenciaSubstituicao = substituicao.IdAusenciaSubstituicao,
                            DataHoraAgendamento = agenda.DataHoraAgendamento,
                            IdAgenda = agenda.IdAgenda,
                            IdDoutorSubstituto = doutorSubstituto.IdDoutor,
                            NomeDoutorSubstituto = doutorSubstituto.Nome
                        })
                        .OrderBy(a => a.DataHoraAgendamento)
                        .ToList();
            }
        }

        /// <summary>
        /// Uma ausência pode ou não ter seu retorno registrado no momento do cadastro, porém o retorno pode ser registrado posteriormente.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool RegistrarRetorno(AusenciaRetornoForm model)
        {
            using (var context = new DatabaseContext())
            {
                if (model.DataHoraFim == DateTime.MinValue) throw new BusinessException("Informe a data de retorno para continuar");

                var entity = GetByKeyFull(model.IdAusencia);
                if (entity == null) throw new BusinessException("Ausência não encontrada");

                if (model.DataHoraFim < entity.DataHoraInicio) throw new BusinessException("A data de retorno não pode ser anterior à data de saída.");

                entity.DataHoraAlteracao = CurrentDateTime;
                entity.IdUsuarioAlteracao = IdUsuarioLogado;
                entity.DataHoraFim = model.DataHoraFim;
                entity.ObservacaoRetorno = model.ObservacaoRetorno;

                context.Entry(entity).State = EntityState.Modified;

                var result = context.SaveChanges() > 0;
                if (result) { RegistrarLogAtividade(entity, PERMISSAO_REGISTRAR_RETORNO); }
                return result;
            }
        }
    }
}
