using System.Net.Http;
using System.Threading.Tasks;

namespace Everest.UnitTests
{
    public class AlwaysThrowsOnSendingAdapter : HttpClientAdapter
    {
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return Task.Factory.StartNew<HttpResponseMessage>(() =>
            {
                throw new DeliberateException();
            });
        }
    }
}