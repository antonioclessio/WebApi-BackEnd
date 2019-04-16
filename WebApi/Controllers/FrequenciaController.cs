using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Frequencia;
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
    [RoutePrefix("api/v1/frequencia")]
    public class FrequenciaController : BaseApiController, IApiController<Frequencia, FrequenciaFilterQuery, FrequenciaForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "15.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new FrequenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Frequência retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do frequência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "15.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new FrequenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Frequência retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do frequência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "15.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] FrequenciaFilterQuery filter)
        {
            using (var repository = new FrequenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de frequência retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os frequência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "15.2, 15.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new FrequenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do frequência retornado com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do frequência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "15.2, 15.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] FrequenciaForm entity)
        {
            using (var repository = new FrequenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Frequência salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do frequência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "15.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new FrequenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Frequência excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um frequência", ex.Message));
                }
            }
        }
        #endregion
    }
}