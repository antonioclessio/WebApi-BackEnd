using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.Paciente;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Exceptions;
using GrupoOrto.ERP.WebApi.Authentication;
using GrupoOrto.ERP.WebApi.Interfaces;
using System.Net;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/paciente")]
    public class PacienteController : BaseApiController, IApiController<Paciente, PacienteFilterQuery, PacienteForm>
    {
        #region # Interface
        [CustomAuthorize(Roles = "11.1")]
        [HttpGet]
        [Route("{key}")]
        public IHttpActionResult GetByKey([FromUri] int key)
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Paciente retornado com sucesso", repository.GetByKey(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do paciente", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "11.1")]
        [HttpGet]
        [Route("{key}/full")]
        public IHttpActionResult GetByKeyFull([FromUri] int key)
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Paciente retornado com sucesso", repository.GetByKeyFull(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do paciente", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "11.1")]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetList([FromUri] PacienteFilterQuery filter)
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de pacientes retornada com sucesso", repository.GetList(filter)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os pacientes", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "11.2, 11.3")]
        [HttpGet]
        [Route("{key}/edit")]
        public IHttpActionResult GetForEdit([FromUri] int key)
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Dados do paciente retornada com sucesso", repository.GetForEdit(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os dados do paciente", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "11.2, 11.3")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] PacienteForm entity)
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Paciente salvo com sucesso", repository.Save(entity)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao salvar os dados da paciente", ex.Message));
                }
            }
        }

        [CustomAuthorize(Roles = "11.4")]
        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult Delete([FromUri] int key)
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Paciente excluído com sucesso", repository.Delete(key)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao excluir um paciente", ex.Message));
                }
            }
        }
        #endregion

        /// <summary>
        /// Retorna a estatística de pacientes que não contém algumas informações cadastradas consideradas mínima, como CPF, data de nascimento dentre outros.
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize(Roles = "11.12")]
        [Route("desatualizados")]
        [HttpGet]
        public IHttpActionResult GetDesatualizados()
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Estatísticas de clientes desatualizados", repository.GetPacientesDesatualizados()));
                }
                catch (BusinessException ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a estatísticas de clientes desatualizados", ex.Message));
                }
            }
        }

        /// <summary>
        /// Retorna a lista de aniversariantes do dia
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize(Roles = "11.13")]
        [Route("aniversariantes")]
        [HttpGet]
        public IHttpActionResult GetAniversariantesDia()
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Lista de aniversariantes retornado", repository.GetAniversariantesDia()));
                }
                catch (BusinessException ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar a lista de aniversariantes", ex.Message));
                }
            }
        }

        /// <summary>
        /// Action destinada à pesquisa de autocomplete.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [CustomAuthorize(Roles = "11.1")]
        [HttpGet]
        [Route("autocomplete")]
        public IHttpActionResult GetAutoComplete([FromUri] string q)
        {
            using (var repository = new PacienteRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Pacientes encontrados", repository.AutoComplete(q)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao pesquisar os pacientes", ex.Message));
                }
            }
        }
    }
}