using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.ContaReceber;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/conta-receber")]
    public class ContaReceberController : BaseApiController, IApiController<ContaReceber, ContaReceberFilterQuery, ContaReceberForm>
    {
        #region # Interface
        //[CustomAuthorize(Roles = "26.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new ContaReceberRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Conta a receber retornada com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da conta a receber", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "26.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new ContaReceberRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Conta a receber retornada com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da conta a receber", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "26.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] ContaReceberFilterQuery filter)
        {
            using (var repository = new ContaReceberRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de contas a receber retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as contas a receber", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "26.2, 3.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new ContaReceberRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados da conta a receber retornada com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da conta a receber", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "26.2, 3.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] ContaReceberForm entity)
        {
            using (var repository = new ContaReceberRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Conta a receber salva com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados da conta a receber", ex.Message));
                }
            }
        }

        //[CustomAuthorize(Roles = "26.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new ContaReceberRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Conta a receber excluída com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir uma conta a receber", ex.Message));
                }
            }
        }
        #endregion

        /// <summary>
        /// Retorna os vencimentos do dia para a clínica logada
        /// </summary>
        /// <returns></returns>
        //[CustomAuthorize(Roles = "3.1")]
        [CustomAuthorize]
        [HttpGet]
        [Route("vencimentos-dia")]
        public IHttpActionResult GetVencimentosDia()
        {
            using (var repository = new ContaReceberRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Vencimentos do dia", repository.GetVencimentosDia()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os vencimentos do dia", ex.Message));
                }
            }
        }
    }
}
