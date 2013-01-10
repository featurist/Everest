using System.Net.Http;
using System.Threading.Tasks;

namespace Everest.SystemNetHttp
{
    public class SystemNetHttpClientAdapter : HttpClientAdapter
    {
        readonly HttpClient _client = new HttpClient();

        public SystemNetHttpClientAdapter()
        {
            _client.DefaultRequestHeaders.Add("Accept", "*/*");
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
             return _client.SendAsync(request);
        }
    }
}
