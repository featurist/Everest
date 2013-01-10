using System;

namespace Everest
{
    public class RequestErrorEventArgs : RequestEventArgs
    {
        private readonly Exception _exception;

        public RequestErrorEventArgs(RequestDetails request, Exception exception) : base(request)
        {
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}