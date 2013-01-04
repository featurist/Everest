using System;

namespace Everest.Status
{
    public class UnexpectedStatusException : Exception
    {
        private readonly int _statusCode;

        public UnexpectedStatusException(int statusCode, string message) : base(message)
        {
            _statusCode = statusCode;
        }

        public int StatusCode
        {
            get { return _statusCode; }
        }
    }
}
