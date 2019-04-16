using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Usuario;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;
using GrupoOrto.ERP.WebApi.Mailers;
using System;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/usuario")]
    public class UsuarioController : BaseApiController, IApiController<Usuario, UsuarioFilterQuery, UsuarioForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "2.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Usuário retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do usuário", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "2.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Usuário retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do usuário", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "2.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] UsuarioFilterQuery filter)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de usuários retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os usuários", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "2.2, 2.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do usuário retornado com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do usuário", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "2.2, 2.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] UsuarioForm entity)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    repository.Save(entity);

                    // Se for um usuário novo, envia o e-mail de boas vindas
                    if (entity.IdUsuario == 0)
                    {
                        IMailer mailer = new Mailer();
                        mailer.NovoUsuario(entity).Send();
                    }

                    return Ok(CreateResponse(true, "Usuário salvo com sucesso"));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados do usuário", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "2.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Usuário excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um usuário", ex.Message));
                }
            }
        }
        #endregion

        [CustomAuthorize(Roles = "2.5")]
        [Route("resetar-senha")]
        [HttpPost]
        public IHttpActionResult PostResetarSenha([FromBody] int id)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    var entity = repository.ResetarSenha(id);

                    IMailer mailer = new Mailer();
                    mailer.ResetarSenha(entity).Send();

                    return Ok(CreateResponse(true, "Senha resetada com sucesso", entity));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao resetar a senha do usuário", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "2.6")]
        [Route("alterar-senha")]
        [HttpPost]
        public IHttpActionResult PostAlterarSenha([FromBody] AlterarSenhaModel model)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    repository.AlterarSenha(model);
                    return Ok(CreateResponse(true, "Senha alterada com sucesso"));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao alterar a senha do usuário", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "2.7")]
        [Route("log-acesso")]
        [HttpGet]
        public IHttpActionResult GetRelatorioLogAcesso([FromUri] RelatorioLogAcessoUsuarioModel model)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Relatório retornado com sucesso", repository.RelatorioLogAcessoUsuario(model)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar o relatório", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "2.8")]
        [Route("log-atividade")]
        [HttpGet]
        public IHttpActionResult GetRelatorioLogAtividade([FromUri] RelatorioLogAtividadeUsuarioModel model)
        {
            using (var repository = new UsuarioRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Relatório retornado com sucesso", repository.RelatorioLogAtividcadeUsuario(model)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar o relatório", ex.Message));
                }
            }
        }
    }
}