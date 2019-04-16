using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.GradeSalas;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/grade-salas")]
    public class GradeSalasController : BaseApiController, IApiController<GradeSalas, GradeSalasFilterQuery, GradeSalasForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "23.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Grade de salas retornada com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da grade de salas", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Grade de salas retornada com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da Grade de salas", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] GradeSalasFilterQuery filter)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de grades de salas retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as gras de salas", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.2, 23.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados da grade de salas retornados com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da grade de salas", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.2, 23.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] GradeSalasForm entity)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Grade de salas salva com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados da grade de salas", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Grade de salas excluída com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir uma grade de salas", ex.Message));
                }
            }
        }
        #endregion

        #region # Grade de Salas
        [CustomAuthorize(Roles = "23.1")]
        [Route("anos")]
        [HttpGet]
        public IHttpActionResult GetAnos()
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Anos que contém grades geradas retornados com sucesso", repository.GetAnos()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os anos", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.5")]
        [Route("gerar/{anoReferencia}")]
        [HttpPost]
        public IHttpActionResult GerarGrade([FromUri] int anoReferencia)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Grade gerada com sucesso", repository.GerarGrade(anoReferencia)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao gerar a grade", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.1")]
        [Route("grade")]
        [HttpGet]
        public IHttpActionResult GetGrade([FromUri] int ano, [FromUri] int mes)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Grade retornada com sucesso", repository.GetGrade(ano, mes)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a grade", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.7")]
        [Route("associacoes/{idGradeSalasMes}/{periodo}")]
        [HttpGet]
        public IHttpActionResult GetAssociacoes([FromUri] int idGradeSalasMes, int periodo)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de doutores associados retornada com sucesso", repository.GetAllAssociacoes(idGradeSalasMes, (byte)periodo)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar todas as associações", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.7")]
        [Route("doutor/{idGradeSalasMes}/{idGradeSalas}/{periodo}")]
        [HttpPost]
        public IHttpActionResult AssociarGradeSalas([FromUri] int idGradeSalasMes, [FromUri] int idGradeSalas, [FromUri] int periodo)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Doutor associado com sucesso", repository.AssociarDoutor(idGradeSalasMes, idGradeSalas, (byte)periodo)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao associar o doutor", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "23.8")]
        [Route("associacoes/{idGradeSalasMes}/{idGradeSalas}/{periodo}")]
        [HttpDelete]
        public IHttpActionResult RemoverAssociacaoGradeSalas([FromUri] int idGradeSalasMes, [FromUri] int idGradeSalas, [FromUri] int periodo)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Associação entre doutor e sala removida com sucesso", repository.RemoverAssociacaoDoutor(idGradeSalasMes, idGradeSalas, (byte)periodo)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao remover a associação entre doutor e sala", ex.Message));
                }
            }
        }
        #endregion

        #region >> Sala Atendimentos
        [CustomAuthorize(Roles = "23.6")]
        [Route("salas")]
        [HttpGet]
        public IHttpActionResult GetAllSala()
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de salas retornada com sucesso", repository.GetAllSalas()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a lista de salas", ex.Message));
                }
            }
        }

        [Route("salas/{key}")]
        [CustomAuthorize(Roles = "23.6")]
        [HttpGet]
        public IHttpActionResult GetSalaByKey([FromUri] int key)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Sala retornada com sucesso", repository.GetSalaByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a sala", ex.Message));
                }
            }
        }

        [Route("salas")]
        [CustomAuthorize(Roles = "23.6")]
        [HttpPost]
        public IHttpActionResult SaveSala([FromBody] SalaAtendimento model)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Sala salva com sucesso", repository.SaveSala(model)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar a sala", ex.Message));
                }
            }
        }

        [Route("salas/{key}")]
        [CustomAuthorize(Roles = "23.6")]
        [HttpDelete]
        public IHttpActionResult DeleteSala([FromUri] int key)
        {
            using (var repository = new GradeSalasRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Sala excluida com sucesso", repository.DeleteSalaByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir a sala", ex.Message));
                }
            }
        }
        #endregion
    }
}