using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Doutor;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Exceptions;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/doutor")]
    public class DoutorController : BaseApiController, IApiController<Doutor, DoutorFilterQuery, DoutorForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "8.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new DoutorRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Doutor retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do doutor", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "8.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new DoutorRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Doutor retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do doutor", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "8.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] DoutorFilterQuery filter)
        {
            using (var repository = new DoutorRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de doutores retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os doutores", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "8.2, 8.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new DoutorRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do doutor retornado com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do doutor", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "8.2, 8.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] DoutorForm entity)
        {
            using (var repository = new DoutorRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Doutor salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do doutor", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "8.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new DoutorRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Doutor excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um doutor", ex.Message));
                }
            }
        }
        #endregion

        #region >> Public Methods
        [CustomAuthorize(Roles = "8.1")]
        [Route("ordenada")]
        [HttpGet]
        public IHttpActionResult GetListOrdenada()
        {
            using (var repo = new DoutorRepository())
            {
                try
                {
                    var listEntity = repo.GetListOrdenada();
                    if (listEntity == null) return Ok(CreateResponse(true, "Nenhum doutor foi encontrado"));
                    return Ok(CreateResponse(true, "Lista de doutores criada com sucesso", listEntity));
                }
                catch (BusinessException ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a lista de doutores", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "8.2, 8.3")]
        [HttpGet]
        [Route("cro")]
        public IHttpActionResult GetByCRO([FromUri] string c)
        {
            using (var repository = new DoutorRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do doutor retornado com sucesso", repository.GetByCRO(c)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do doutor", ex.Message));
                }
            }
        }
        #endregion
    }
}