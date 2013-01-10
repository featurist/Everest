using System;

namespace Everest
{
    public class ResponseEventArgs : EventArgs
    {
        private readonly ResponseDetails _response;

        public ResponseEventArgs(ResponseDetails response)
        {
            _response = response;
        }

        public ResponseDetails Response
        {
            get { return _response; }
        }
    }
}