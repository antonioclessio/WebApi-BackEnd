using GrupoOrto.ERP.Business.Repositories;
using GrupoOrto.ERP.Entities.Entity.LocalizacaoGeografica;
using GrupoOrto.ERP.WebApi.Authentication;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GrupoOrto.ERP.WebApi.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/address")]
    public class LocalizacaoGeograficaController : BaseApiController
    {
        [Route("")]
        [HttpGet]
        public IHttpActionResult GetByCEP(string c)
        {
            using (var repository = new LocalizacaoGeograficaRepository())
            {
                try
                {
                    var entity = repository.GetByCEP(c);
                    if (entity != null) return Ok(CreateResponse(true, "Endereço retornado com sucesso", entity));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", "Token token=1a9518a9fbfe09427853e7e7c7f86843");
                    var response = client.GetAsync("http://www.cepaberto.com/api/v2/ceps.json?cep=" + c).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var viewResponse = JsonConvert.DeserializeObject<CEPAbertoResult>(response.Content.ReadAsStringAsync().Result);
                        if (viewResponse.cep == null)
                            return Content(HttpStatusCode.NotFound, CreateResponse(true, "Endereço não encontrado"));

                        entity = repository.SaveFromCEPAberto(viewResponse);

                        return Ok(CreateResponse(true, "Endereço retornado com sucesso", entity));
                    }

                    return Content(HttpStatusCode.NotFound, CreateResponse(true, "Endereço não encontrado"));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar o endereço", ex.Message));
                }
            }
        }

        [Route("estados")]
        [HttpGet]
        public IHttpActionResult GetEstados()
        {
            using (var repository = new LocalizacaoGeograficaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Estados retornados com sucesso", repository.GetEstados()));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar os estados", ex.Message));
                }
            }
        }

        [Route("cidades/{idEstado}/{text}")]
        [HttpGet]
        public IHttpActionResult GetCidades(int idEstado, string text = null)
        {
            using (var repository = new LocalizacaoGeograficaRepository())
            {
                try
                {
                    return Ok(CreateResponse(true, "Cidades retornadas com sucesso", repository.GetCidades(idEstado, text)));
                }
                catch (System.Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, CreateResponse(false, "Erro ao retornar as cidades", ex.Message));
                }
            }
        }
    }
}