using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Everest.UnitTests
{
    public class AlwaysThrowsOnSendingAdapter : HttpClientAdapter
    {
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            throw new Exception("oopsie");
        }
    }
}