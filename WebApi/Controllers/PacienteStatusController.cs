using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.PacienteStatus;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/paciente-status")]
    public class PacienteStatusController : BaseApiController, IApiController<PacienteStatus, PacienteStatusFilterQuery, PacienteStatusForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "14.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new PacienteStatusRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Status do cliente retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do status do cliente", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "14.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new PacienteStatusRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Status do cliente  retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do status do cliente ", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "14.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] PacienteStatusFilterQuery filter)
        {
            using (var repository = new PacienteStatusRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de status do cliente  retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os status do cliente", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "14.2, 14.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new PacienteStatusRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do status do cliente  retornados com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do status do cliente", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "14.2, 14.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] PacienteStatusForm entity)
        {
            using (var repository = new PacienteStatusRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Status do cliente salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do status do cliente", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "14.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new PacienteStatusRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Status do cliente excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um status do cliente", ex.Message));
                }
            }
        }
        #endregion
    }
}