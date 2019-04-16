using WebApi.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace WebApi.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        protected ActionResponseModel CreateResponse(bool success)
        {
            return CreateResponse(success, null, null, null);
        }

        protected ActionResponseModel CreateResponse(bool success, string message)
        {
            return CreateResponse(success, message, null, null);
        }

        protected ActionResponseModel CreateResponse(bool success, string message, object data)
        {
            return CreateResponse(success, message, data, null);
        }

        protected ActionResponseModel CreateResponse(bool success, string message, object data, List<string> validationMessages)
        {
            return new ActionResponseModel
            {
                Success = success,
                Message = message,
                Data = data,
                Validations = validationMessages
            };
        }
    }
}