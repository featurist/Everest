using System;
using System.Net.Http;
using Everest.Content;
using Everest.Pipeline;

namespace Everest
{
    public interface Resource
    {
        Uri Url { get; }
        Response Send(HttpMethod method, string uri, BodyContent body, params PipelineOption[] overridingPipelineOptions);
        Resource With(params PipelineOption[] options);
    }
}