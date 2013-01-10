using System;
using System.Net.Http;

namespace Everest.SystemNetHttp
{
    public class HttpRequestMessageRequestDetails : RequestDetails
    {
        private readonly HttpRequestMessage _message;

        public HttpRequestMessageRequestDetails(HttpRequestMessage message)
        {
            _message = message;
        }

        public string Method { get { return _message.Method.ToString(); } }
        public Uri RequestUri { get { return _message.RequestUri; } }
    }
}
