using System.Net.Http;

namespace Everest.Pipeline
{
    internal interface PipelineElement
    {
        void ApplyToRequest(HttpRequestMessage request, PipelineOptions pipelineOptions);
        void ApplyToResponse(HttpResponseMessage response, PipelineOptions pipelineOptions);
    }
}