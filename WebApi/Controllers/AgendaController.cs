using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Agenda;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using GrupoOrto.ERP.WebApi.Authentication;
using System;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/agenda")]
    public class AgendaController : BaseApiController
    {
        #region # Agendamentos
        [CustomAuthorize(Roles = "13.1")]
        [Route("periodo")]
        [HttpGet]
        public IHttpActionResult GetAgendamentosPorperiodo(
            [FromUri] int idDoutor,
            [FromUri] DateTime dataInicio,
            [FromUri] DateTime dataFim)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de agendamentos por período do doutor na data informada", repository.RetornarPorPeriodo(idDoutor, dataInicio, dataFim)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a lista de agendamentos por período do doutor na data informada", ex.Message));
                }
            }
        }

        /// <summary>
        /// Retorna os agendamentos acontecidos em uma determinada data
        /// </summary>
        /// <param name="data">Data de referência</param>
        /// <returns></returns>
        [CustomAuthorize(Roles = "13.13")]
        [Route("agendamentos-dia")]
        [HttpGet]
        public IHttpActionResult GetAgendamentosDia([FromUri] DateTime? data = null)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de agendamentos do dia", repository.GetAgendamentosDia(data)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a lista de agendamentos do dia", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.1")]
        [Route("{key}")]
        [HttpGet]
        public IHttpActionResult GetAgendamentoPai([FromUri] int key)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Agendamento retornado com sucesso", repository.GetAgendamentoPai(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar o agendamento", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.5")]
        [Route("")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] AgendaForm model)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Agendamento realizado com sucesso", repository.SalvarAgendamento(model)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar o agendamento", ex.Message));
                }
            }
        }
        #endregion

        #region # Lembretes
        /// <summary>
        /// Retorna os lembretes de agendamentos criados pelo usuário logado em uma determinada data.
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize(Roles = "13.14")]
        [Route("lembretes")]
        [HttpGet]
        public IHttpActionResult Lembrete()
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de lembretes", repository.GetLembretes()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a lista de lembretes", ex.Message));
                }
            }
        }
        #endregion

        #region # Doutor
        /// <summary>
        /// Retorna os agendamentos acontecidos em uma determinada data
        /// </summary>
        /// <param name="data">Data de referência</param>
        /// <returns></returns>
        [CustomAuthorize(Roles = "13.1")]
        [Route("")]
        [HttpGet]
        public IHttpActionResult GetAgendaDoutor([FromUri] int idDoutor, [FromUri] DateTime data)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de agendamentos do doutor na data informada", repository.GetAgendaDoutor(idDoutor, data)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a lista de agendamentos do doutor na data informada", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.15")]
        [Route("cancelar-horario")]
        [HttpPost]
        public IHttpActionResult CancelarHorario([FromBody] CancelarHorarioModel entity)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "O(s) horário(s) do doutor foi(ram) canceladas", repository.CancelarHorarioAgendaDoutor(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao cancelar o horário do doutor", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.18")]
        [Route("liberar-horario")]
        [HttpPost]
        public IHttpActionResult LiberarHorario([FromBody] int key)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "O(s) horário(s) do doutor foi(ram) liberados", repository.LiberarHorarioAgendaDoutor(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao liberar o horário do doutor", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.12")]
        [Route("excluir-horario")]
        [HttpPost]
        public IHttpActionResult RegistrarExcecaoAgendaDoutor([FromBody] Doutor_ExcecaoAgenda entity)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "O(s) horário(s) do doutor foi(ram) excluidos", repository.RegistrarExcecaoAgendaDoutor(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir o horário do doutor", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "8.8")]
        [Route("total-agendamento-diario")]
        [HttpGet]
        public IHttpActionResult GetRelatorioTotalAgendamentosDiarioDoutor([FromUri] RelatorioTotalAgendamentosDiarioDoutorModel model)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Relatório retornado com sucesso", repository.RelatorioTotalAgendamentosDiarioDoutor(model)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar o relatório", ex.Message));
                }
            }
        }
        #endregion

        #region # Confirmações
        [CustomAuthorize(Roles = "13.16")]
        [Route("confirmacao")]
        [HttpGet]
        public IHttpActionResult GetConfirmacao([FromUri] int idAgenda)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Confirmações retornada com sucesso", repository.GetConfirmacoes(idAgenda)));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as confirmações", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.16")]
        [Route("confirmacao")]
        [HttpPost]
        public IHttpActionResult RegistrarConfirmacao([FromBody] Agenda_Confirmacao model)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Confirmação registrada com sucesso", repository.RegistrarConfirmacao(model)));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao registrar a confirmação", ex.Message));
                }
            }
        }
        #endregion

        #region # Frequencias
        [CustomAuthorize(Roles = "13.6")]
        [Route("registrar-chegada")]
        [HttpPost]
        public IHttpActionResult RegistrarChegada([FromBody] int idAgenda)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Chegada registrada com sucesso", repository.RegistrarChegada(idAgenda)));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao registrar a chegada do cliente.", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.17")]
        [Route("registrar-atendimento")]
        [HttpPost]
        public IHttpActionResult RegistrarAtendimento([FromBody] int idAgenda)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Atendimento iniciado com sucesso", repository.RegistrarAtendimento(idAgenda)));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao registrar o inicio do atendimento", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.8")]
        [Route("finalizar-atendimento")]
        [HttpPost]
        public IHttpActionResult FinalizarAtendimento([FromBody] int idAgenda)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Atendimento finalizado com sucesso", repository.FinalizarAtendimento(idAgenda)));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao registrar o fim do atendimento", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.9")]
        [Route("desmarcar-atendimento")]
        [HttpPost]
        public IHttpActionResult DesmarcarAtendimento([FromBody] DesmarcarAgendamentoModel model)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Atendimento desmarcado com sucesso", repository.DesmarcarAtendimento(model)));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao desmarcar o atendimento", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.10")]
        [Route("registrar-falta")]
        [HttpPost]
        public IHttpActionResult RegistrarFalta([FromBody] int idAgenda)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Falta registrada com sucesso", repository.RegistrarFalta(idAgenda)));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao registrar a falta", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "13.11")]
        [Route("reagendar-atendimento")]
        [HttpPost]
        public IHttpActionResult ReagendarAtendimento([FromBody] ReagendarAtendimentoModel model)
        {
            using (var repository = new AgendaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Atendimento reagendado com sucesso", repository.ReagendarAtendimento(model)));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao reagendar o atendimento", ex.Message));
                }
            }
        }
        #endregion
    }
}