using System;

namespace Everest.Status
{
    public class UnexpectedStatusException : Exception
    {
        public UnexpectedStatusException(string message) : base(message)
        {
        }
    }
}
