using Everest.Pipeline;

namespace Everest
{
    public interface HttpClientAdapterFactory
    {
        HttpClientAdapter CreateClient(PipelineOptions options);
    }
}
