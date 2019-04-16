using System.Collections.Generic;

namespace WebApi.Models
{
    public class ActionResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Validations { get; set; }
        public object Data { get; set; }
    }
}