using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Curso;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/curso")]
    public class CursoController : BaseApiController, IApiController<Curso, CursoFilterQuery, CursoForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "6.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new CursoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Curso retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do curso", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "6.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new CursoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Curso retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do curso", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "6.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] CursoFilterQuery filter)
        {
            using (var repository = new CursoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de cursos retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os cursos", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "6.2, 6.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new CursoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do curso retornados com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do curso", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "6.2, 6.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] CursoForm entity)
        {
            using (var repository = new CursoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Curso salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do curso", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "6.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new CursoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Curso excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um curso", ex.Message));
                }
            }
        }
        #endregion
    }
}