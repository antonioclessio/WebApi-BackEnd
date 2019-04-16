using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.WebApi.Authentication;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/cockpit")]
    public class CockpitController : BaseApiController
    {
        [CustomAuthorize(Roles = "39.1")]
        [Route("resultado-diario")]
        [HttpGet]
        public IHttpActionResult ResultadoDiario(
            [FromUri] int ano,
            [FromUri] int mes,
            [FromUri] bool consolidado,
            [FromUri] int? idClinica)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.ResultadoDiario(ano, mes, consolidado, null, idClinica)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "39.1")]
        [Route("resultadodiario/salas")]
        [HttpGet]
        public IHttpActionResult ResultadoDiario_Salas(
            [FromUri] int ano,
            [FromUri] int mes,
            [FromUri] int idClinica)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.ResultadoDiario_Salas(ano, mes, idClinica)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "32.1")]
        [Route("pagina28")]
        [HttpGet]
        public IHttpActionResult ResultadoDiario_Pagina28(
            [FromUri] int ano,
            [FromUri] int mes)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.ResultadoDiario_Pagina28(ano, mes)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "36.1")]
        [Route("inadimplencia")]
        [HttpGet]
        public IHttpActionResult ResultadoDiario_Inadimplencia(
            [FromUri] int ano,
            [FromUri] int mes)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.ResultadoDiario_Inadimplencia(ano, mes)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "34.1")]
        [Route("analise")]
        [HttpGet]
        public IHttpActionResult ResultadoDiario_Analise(
            [FromUri] int ano,
            [FromUri] int mes)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.ResultadoDiario_Analise(ano, mes)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "35.1")]
        [Route("saldo")]
        [HttpGet]
        public IHttpActionResult ResultadoDiario_Saldo(
                    [FromUri] int ano,
                    [FromUri] int mes)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.ResultadoDiario_Saldo(ano, mes)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "37.1")]
        [Route("analise-implante")]
        [HttpGet]
        public IHttpActionResult ResultadoDiario_AnaliseImplante(
                    [FromUri] int ano,
                    [FromUri] int mes)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.ResultadoDiario_AnaliseImplante(ano, mes)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "33.1")]
        [Route("faturamento")]
        [HttpGet]
        public IHttpActionResult ResultadoDiario_Faturamento(
            [FromUri] int ano,
            [FromUri] int mes)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.ResultadoDiario_Faturamento(ano, mes)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "38.1")]
        [Route("dados-dentistas")]
        [HttpGet]
        public IHttpActionResult DadosDentistas(
            [FromUri] int idDoutor,
            [FromUri] int anoReferencia,
            [FromUri] int? idClinica)
        {
            using (var repository = new CockpitRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Resultado do relatório", repository.DadosDentistas(idDoutor, anoReferencia, idClinica)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do relatório", ex.Message));
                }
            }
        }
    }
}