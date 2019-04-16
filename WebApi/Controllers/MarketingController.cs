using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Marketing;
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
    [RoutePrefix("api/v1/marketing")]
    public class MarketingController : BaseApiController, IApiController<Marketing, MarketingFilterQuery, MarketingForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "12.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new MarketingRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Marketing retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do marketing", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "12.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new MarketingRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Marketing retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do marketing", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "12.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] MarketingFilterQuery filter)
        {
            using (var repository = new MarketingRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de Marketinges retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os marketinges", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "12.2, 12.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new MarketingRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do marketing retornado com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do marketing", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "12.2, 12.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] MarketingForm entity)
        {
            using (var repository = new MarketingRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Marketing salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do marketing", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "12.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new MarketingRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Marketing excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um marketing", ex.Message));
                }
            }
        }
        #endregion
    }
}