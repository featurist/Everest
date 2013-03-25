using Everest.Pipeline;

namespace Everest.UnitTests.Fakes
{
    public class StubAdapterFactory : HttpClientAdapterFactory
    {
        private readonly HttpClientAdapter _adapter;

        public StubAdapterFactory(HttpClientAdapter adapter)
        {
            _adapter = adapter;
        }

        public HttpClientAdapter CreateClient(PipelineOptions options)
        {
            return _adapter;
        }
    }
}