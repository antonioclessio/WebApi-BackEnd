using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Ocorrencia;
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
    [RoutePrefix("api/v1/ocorrencia")]
    public class OcorrenciaController : BaseApiController, IApiController<Ocorrencia, OcorrenciaFilterQuery, OcorrenciaForm>
    {
        #region # Interface
        //[CustomAuthorize(Roles = "11.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new OcorrenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Profissão retornada com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da profissão", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "11.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new OcorrenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Profissão retornada com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da profissão", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "11.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] OcorrenciaFilterQuery filter)
        {
            using (var repository = new OcorrenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de profissões retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as profissões", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "11.2, 11.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new OcorrenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados da profissão retornados com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da profissão", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "11.2, 11.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] OcorrenciaForm entity)
        {
            using (var repository = new OcorrenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Profissão salva com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados da profissão", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "11.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new OcorrenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Profissão excluída com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir uma profissão", ex.Message));
                }
            }
        }
        #endregion
    }
}