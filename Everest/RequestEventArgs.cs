using System;

namespace Everest
{
    public class RequestEventArgs : EventArgs
    {
        private readonly RequestDetails _request;

        public RequestEventArgs(RequestDetails request)
        {
            _request = request;
        }

        public RequestDetails Request
        {
            get { return _request; }
        }
    }
}