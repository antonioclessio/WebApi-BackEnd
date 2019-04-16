using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Especialidade;
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
    [RoutePrefix("api/v1/especialidade")]
    public class EspecialidadeController : BaseApiController, IApiController<Especialidade, EspecialidadeFilterQuery, EspecialidadeForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "0")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new EspecialidadeRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Especialidade retornada com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da especialidade", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "0")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new EspecialidadeRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Especialidade retornada com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da especialidade", ex.Message));
                }
            }
        }

        /// <summary>
        /// Esta action é controlada pela permissão de leitura de doutor
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [CustomAuthorize(Roles = "8.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] EspecialidadeFilterQuery filter)
        {
            using (var repository = new EspecialidadeRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de especialidades retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as especialidades", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "0")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new EspecialidadeRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados da especialidade retornados com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados da especialidade", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "0")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] EspecialidadeForm entity)
        {
            using (var repository = new EspecialidadeRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Especialidade salva com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados da especialidade", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "0")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new EspecialidadeRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Especialidade excluída com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir uma especialidade", ex.Message));
                }
            }
        }
        #endregion
    }
}