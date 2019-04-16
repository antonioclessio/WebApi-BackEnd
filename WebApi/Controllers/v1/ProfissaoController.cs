using Business.Repositories.v1;
using Entities.Entity.Profissao;
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
    [RoutePrefix("api/v1/profissao")]
    public class ProfissaoController : BaseApiController, IApiController<Profissao, ProfissaoFilterQuery, ProfissaoForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "18.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new ProfissaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados retornados com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "18.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new ProfissaoRepository())
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

        [CustomAuthorize(Roles = "18.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] ProfissaoFilterQuery filter)
        {
            using (var repository = new ProfissaoRepository())
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

        [CustomAuthorize(Roles = "18.2, 18.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new ProfissaoRepository())
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

        [CustomAuthorize(Roles = "18.2, 18.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] ProfissaoForm entity)
        {
            using (var repository = new ProfissaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados salvos com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "18.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new ProfissaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Registro excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir o registro", ex.Message));
                }
            }
        }
        #endregion

        [CustomAuthorize(Roles = "18.1")]
        [HttpGet]
        [Route("autocomplete")]
        public IHttpActionResult GetAutoComplete([FromUri] string q)
        {
            using (var repository = new ProfissaoRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados retornados com sucesso", repository.AutoComplete(q)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados", ex.Message));
                }
            }
        }
    }
}