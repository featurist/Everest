using System;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Threading.Tasks;
using Everest.Redirection;

namespace Everest.SystemNetHttp
{
    public class SystemNetHttpClientAdapter : HttpClientAdapter
    {
        private readonly AutoRedirect _autoRedirect;
        private readonly HttpClient _client;

        public SystemNetHttpClientAdapter(AdapterOptions options)
        {
            _autoRedirect = options.AutoRedirect;
            var handler = new WebRequestHandler
            {
                AllowAutoRedirect = !(AutoRedirect.AutoRedirectAndForwardAuthorizationHeader.Equals(options.AutoRedirect) ||
                                      AutoRedirect.DoNotAutoRedirect.Equals(options.AutoRedirect)),
                UseCookies = false,
            };

            if (options.CachePolicy.Cache)
            {
                handler.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Default);
            }

            if (options.AcceptEncoding.AcceptGzipAndDeflate)
            {
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }

            _client = new HttpClient(handler);

            if (options.Timeout != null)
            {
                _client.Timeout = options.Timeout.TimeSpan;
            }
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = _client.SendAsync(request).Result;
            if (ShouldManuallyRedirect(response))
            {
                return _client.SendAsync(CreateRedirectRequest(request, response));
            }
            return Task.Factory.StartNew(() => response);
        }

        private bool ShouldManuallyRedirect(HttpResponseMessage response)
        {
            return (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently)
                   && AutoRedirect.AutoRedirectAndForwardAuthorizationHeader.Equals(_autoRedirect);
        }

        private static HttpRequestMessage CreateRedirectRequest(HttpRequestMessage request, HttpResponseMessage response)
        {
            var newRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(request.RequestUri, response.Headers.Location));
            foreach (var header in request.Headers)
            {
                newRequest.Headers.Add(header.Key, header.Value);
            }
            return newRequest;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public HttpClient Client
        {
            get { return _client; }
        }
    }
}
