using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Confirmacao;
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
    [RoutePrefix("api/v1/confirmacao")]
    public class ConfirmacaoController : BaseApiController, IApiController<Confirmacao, ConfirmacaoFilterQuery, ConfirmacaoForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "10.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new ConfirmacaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Confirmação retornada com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da confirmação", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "10.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new ConfirmacaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Confirmação retornada com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da confirmação", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "10.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] ConfirmacaoFilterQuery filter)
        {
            using (var repository = new ConfirmacaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de confirmações retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as confirmações", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "10.2, 10.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new ConfirmacaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados da confirmação retornada com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da confirmação", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "10.2, 10.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] ConfirmacaoForm entity)
        {
            using (var repository = new ConfirmacaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Confirmação salva com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados da confirmação", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "10.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new ConfirmacaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Confirmação excluída com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir a confirmação", ex.Message));
                }
            }
        }
        #endregion
    }
}