using System;

namespace Everest.Status
{
    public class UnexpectedStatusException : Exception
    {
        public UnexpectedStatusException(int statusCode, string message) : base(message)
        {
        }
    }
}
