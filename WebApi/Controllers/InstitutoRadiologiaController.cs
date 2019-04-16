using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.InstitutoRadiologia;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/instituto-radiologia")]
    public class InstitutoRadiologiaController : BaseApiController, IApiController<InstitutoRadiologia, InstitutoRadiologiaFilterQuery, InstitutoRadiologiaForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "16.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new InstitutoRadiologiaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Instituto retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "16.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new InstitutoRadiologiaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados retornados com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "16.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] InstitutoRadiologiaFilterQuery filter)
        {
            using (var repository = new InstitutoRadiologiaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados retornados com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "16.2, 16.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new InstitutoRadiologiaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados retornados com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "16.2, 16.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] InstitutoRadiologiaForm entity)
        {
            using (var repository = new InstitutoRadiologiaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados retornados com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "16.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new InstitutoRadiologiaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados excluídos com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir os dados", ex.Message));
                }
            }
        }
        #endregion
    }
}