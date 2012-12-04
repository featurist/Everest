using System.Collections.Generic;
using System.Net.Http;
using Everest.Content;
using Everest.Pipeline;

namespace Everest
{
    public interface Resource
    {
        Response Get(string uri, IDictionary<string, string> headers, params PipelineOption[] options);
        Response Post(string url, string body, params PipelineOption[] pipelineOptions);
        Response Put(string uri, string body, params PipelineOption[] pipelineOptions);
        Response Put(string uri, BodyContent body, params PipelineOption[] pipelineOptions);
        Response Head(string uri, params PipelineOption[] pipelineOptions);
        Response Delete(string uri, params PipelineOption[] pipelineOptions);
        Response Send(HttpMethod method, string uri, BodyContent body, params PipelineOption[] overridingPipelineOptions);
        Response Get(string uri, params PipelineOption[] pipelineOptions);
    }
}