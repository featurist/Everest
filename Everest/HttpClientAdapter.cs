using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Everest
{
    public interface HttpClientAdapter : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
