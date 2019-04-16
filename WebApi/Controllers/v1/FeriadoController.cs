using Business.Repositories.v1;
using Entities.Entity.Feriado;
using Entities.Entity.Table;
using WebApi.Authentication;
using WebApi.Interfaces.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers.v1
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/feriado")]
    public class FeriadoController : BaseApiController, IApiController<Feriado, FeriadoFilterQuery, FeriadoForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "7.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new FeriadoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Feriado retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do Feriado", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "7.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new FeriadoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Feriado retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do Feriado", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "7.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] FeriadoFilterQuery filter)
        {
            using (var repository = new FeriadoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de Feriadoes retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os Feriadoes", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "7.2, 7.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new FeriadoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do Feriado retornado com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do Feriado", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "7.2, 7.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] FeriadoForm entity)
        {
            using (var repository = new FeriadoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Feriado salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do Feriado", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "7.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new FeriadoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Feriado excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um Feriado", ex.Message));
                }
            }
        }
        #endregion
    }
}