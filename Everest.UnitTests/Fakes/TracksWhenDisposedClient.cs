using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Everest.UnitTests.Fakes
{
    public class TracksWhenDisposedClient : HttpClientAdapter
    {
        public readonly List<TracksWhenDisposedResponseMessage> Responses = new List<TracksWhenDisposedResponseMessage>();

        public TracksWhenDisposedClient()
        {
            DisposeCount = 0;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = new TracksWhenDisposedResponseMessage();
            Responses.Add(response);
            return Task.Factory.StartNew(() => (HttpResponseMessage)response);
        }

        public void Dispose()
        {
            DisposeCount++;
        }

        public int DisposeCount { get; private set; }
    }
}