using System.Net.Http;
using System.Threading.Tasks;

namespace Everest
{
    public interface HttpClientAdapter
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
