using Business.Repositories.v1;
using Entities.Entity.UsuarioGrupo;
using Entities.Entity.Table;
using WebApi.Authentication;
using WebApi.Interfaces.V1;
using System.Net;
using System.Web.Http;

namespace WebApi.Controllers.v1
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/usuario-grupo")]
    public class UsuarioGrupoController : BaseApiController, IApiController<UsuarioGrupo, UsuarioGrupoFilterQuery, UsuarioGrupoForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "1.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new UsuarioGrupoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "UsuarioGrupo retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do UsuarioGrupo", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "1.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new UsuarioGrupoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "UsuarioGrupo retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do UsuarioGrupo", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "1.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] UsuarioGrupoFilterQuery filter)
        {
            using (var repository = new UsuarioGrupoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de UsuarioGrupoes retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os UsuarioGrupoes", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "1.2, 1.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new UsuarioGrupoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do UsuarioGrupo retornado com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do UsuarioGrupo", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "1.2, 1.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] UsuarioGrupoForm entity)
        {
            using (var repository = new UsuarioGrupoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "UsuarioGrupo salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do UsuarioGrupo", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "1.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new UsuarioGrupoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "UsuarioGrupo excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um UsuarioGrupo", ex.Message));
                }
            }
        }
        #endregion
    }
}