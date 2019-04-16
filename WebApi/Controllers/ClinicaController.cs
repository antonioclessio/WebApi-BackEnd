using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Clinica;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/clinica")]
    public class ClinicaController : BaseApiController, IApiController<Clinica, ClinicaFilterQuery, ClinicaForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "3.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new ClinicaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Clínica retornada com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da clínica", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "3.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new ClinicaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Clínica retornada com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da clínica", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "3.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] ClinicaFilterQuery filter)
        {
            using (var repository = new ClinicaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista d clínicas retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as clínicas", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "3.2, 3.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new ClinicaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados da clínica retornada com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da clínica", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "3.2, 3.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] ClinicaForm entity)
        {
            using (var repository = new ClinicaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Clínica salva com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados da clínica", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "3.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new ClinicaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Clínica excluída com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir uma clínica", ex.Message));
                }
            }
        }
        #endregion

        /// <summary>
        /// Retorna a lista de clínicas. Este método é público, não havendo necessidade de autenticação.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("public")]
        public IHttpActionResult GetPublic()
        {
            using (var repository = new ClinicaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de Clínicas", repository.GetListPublic()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as clínicas", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "3.1")]
        [Route("siglas")]
        [HttpGet]
        public IHttpActionResult GetClinicaSigla()
        {
            using (var repo = new ClinicaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de clínicas", repo.GetListaSigla()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as clínicas", ex.Message));
                }
            }
        }
    }
}
