using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.WebApi.Authentication;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/satisfacao")]
    public class SatisfacaoController : BaseApiController
    {
        [Route("")]
        [HttpGet]
        public IHttpActionResult GetSatisfacaoPorAgendamento([FromUri] int idPaciente, int? idAgenda)
        {
            using (var repository = new SatisfacaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Satisfação retornada com sucesso", repository.GetSatisfacaoPorAgendamento(idPaciente, idAgenda)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da satisfação", ex.Message));
                }
            }
        }
    }
}