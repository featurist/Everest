namespace Everest
{
    public class ResponseEventArgs : RequestEventArgs
    {
        private readonly ResponseDetails _response;

        public ResponseEventArgs(RequestDetails request, ResponseDetails response) : base(request)
        {
            _response = response;
        }

        public ResponseDetails Response
        {
            get { return _response; }
        }
    }
}