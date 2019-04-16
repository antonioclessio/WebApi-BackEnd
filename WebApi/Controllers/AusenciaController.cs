using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Ausencia;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Exceptions;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/ausencia")]
    public class AusenciaController : BaseApiController, IApiController<Ausencia, AusenciaFilterQuery, AusenciaForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "9.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new AusenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Ausência retornada com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da ausência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "9.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new AusenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Ausência retornada com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da ausência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "9.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] AusenciaFilterQuery filter)
        {
            using (var repository = new AusenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de ausências retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as ausências", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "9.2, 9.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new AusenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados da ausência retornados com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da ausência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "9.2, 9.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] AusenciaForm entity)
        {
            using (var repository = new AusenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Ausência salva com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados da ausência", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "9.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new AusenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Ausência excluída com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir uma ausência", ex.Message));
                }
            }
        }
        #endregion

        [CustomAuthorize(Roles = "9.1")]
        [Route("{key}/substituicoes")]
        [HttpGet]
        public IHttpActionResult GetSubstituicoes([FromUri] int key)
        {
            using (var repository = new AusenciaRepository())
            {
                try
                {
                    var listEntity = repository.GetSubstituicoes(key);
                    if (listEntity == null) return Ok(CreateResponse(true, "Nenhuma substituição foi encontrada"));
                    return Ok(CreateResponse(true, "Lista de substituições criada com sucesso", listEntity));
                }
                catch (BusinessException ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as substituições", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "9.5")]
        [Route("retorno")]
        [HttpPost]
        public IHttpActionResult RegistrarRetorno([FromBody] AusenciaRetornoForm entity)
        {
            using (var repository = new AusenciaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "O retorno foi registrado", repository.RegistrarRetorno(entity)));
                }
                catch (BusinessException ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao registrar o retorno", ex.Message));
                }
            }
        }
    }
}