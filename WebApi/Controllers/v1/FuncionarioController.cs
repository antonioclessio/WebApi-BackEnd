using Business.Repositories.v1;
using Entities.Entity.Funcionario;
using Entities.Entity.Table;
using WebApi.Authentication;
using WebApi.Interfaces.V1;
using System.Net;
using System.Web.Http;

namespace WebApi.Controllers.v1
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/funcionario")]
    public class FuncionarioController : BaseApiController, IApiController<Funcionario, FuncionarioFilterQuery, FuncionarioForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "5.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new FuncionarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Funcionario retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do Funcionario", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "5.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new FuncionarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Funcionario retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do Funcionario", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "5.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] FuncionarioFilterQuery filter)
        {
            using (var repository = new FuncionarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de Funcionarioes retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os Funcionarioes", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "5.2, 5.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new FuncionarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do Funcionario retornado com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do Funcionario", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "5.2, 5.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] FuncionarioForm entity)
        {
            using (var repository = new FuncionarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Funcionario salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do Funcionario", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "5.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new FuncionarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Funcionario excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um Funcionario", ex.Message));
                }
            }
        }
        #endregion
    }
}