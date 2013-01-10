using System.Net.Http;

namespace Everest.SystemNetHttp
{
    public class HttpResponseMessageResponseDetails : ResponseDetails
    {
        private readonly RequestDetails _request;
        private readonly HttpResponseMessage _response;

        public HttpResponseMessageResponseDetails(RequestDetails request, HttpResponseMessage response)
        {
            _request = request;
            _response = response;
        }

        public RequestDetails Request
        {
            get { return _request; }
        }

        public int Status
        {
            get { return (int) _response.StatusCode; }
        }
    }
}