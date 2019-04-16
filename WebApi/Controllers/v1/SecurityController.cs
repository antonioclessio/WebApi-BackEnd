using Business.Repositories.v1;
using Entities.Entity.Security;
using WebApi.Authentication;
using System.Net;
using System.Web.Http;

namespace WebApi.Controllers.v1
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/security")]
    public class SecurityController : BaseApiController
    {
        /// <summary>
        /// Realiza a verificação do usuário e autenticação
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("authentication")]
        public IHttpActionResult PostAuthentication([FromBody] AuthenticationQuery query)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Usuário Autenticado", repository.Authentication(query)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao realizar a autenticação", ex.Message));
                }
            }
        }

        /// <summary>
        /// Retorna a estrutura da aplicação com base n usuário autenticado
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("application")]
        public IHttpActionResult GetApplicationStructure()
        {
            using (var repository = new ApplicationRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Application Structure carregado", repository.GetApplicationStructure()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a strutura da aplicação", ex.Message));
                }
            }
        }

        /// <summary>
        /// Retorna todas as aplicações que compõe o sistema, bem como suas permissões.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("aplicacoes")]
        public IHttpActionResult GetAplicacao()
        {
            using (var repository = new ApplicationRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de aplicações carregadas", repository.GetAplicacoes()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as aplicações", ex.Message));
                }
            }
        }
    }
}
