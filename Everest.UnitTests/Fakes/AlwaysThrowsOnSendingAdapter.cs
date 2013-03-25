using System.Net.Http;
using System.Threading.Tasks;
using Everest.Pipeline;

namespace Everest.UnitTests.Fakes
{
    public class AlwaysThrowsOnSendingAdapter : HttpClientAdapter, HttpClientAdapterFactory
    {
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return Task.Factory.StartNew<HttpResponseMessage>(() =>
            {
                throw new DeliberateException();
            });
        }

        public HttpClientAdapter CreateClient(PipelineOptions options)
        {
            return new AlwaysThrowsOnSendingAdapter();
        }

        public void Dispose()
        {
        }
    }
}