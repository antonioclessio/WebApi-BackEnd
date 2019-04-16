using System;

namespace Entities.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException() : base() { }
        public BusinessException(string message) : base(message) { }
        public BusinessException(Exception ex) : base(ex.Message)
        {

        }
    }
}
