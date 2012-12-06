using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Everest.Content;
using Everest.Headers;
using Everest.Pipeline;

namespace Everest
{
    public static class ResourceApi
    {
        public static Response Get(this Resource resource, string uri, params PipelineOption[] pipelineOptions)
        {
            return resource.Send(HttpMethod.Get, uri, null, pipelineOptions);
        }

        public static Response Get(this Resource resource, string uri, IDictionary<string, string> headers, params PipelineOption[] options)
        {
            return resource.Get(uri, options.Union(new[] { new SetRequestHeaders(headers) }).ToArray());
        }

        public static Response Post(this Resource resource, string url, string body, params PipelineOption[] pipelineOptions)
        {
            return resource.Send(HttpMethod.Post, url, new StringBodyContent(body), pipelineOptions);
        }

        public static Response Put(this Resource resource, string uri, string body, params PipelineOption[] pipelineOptions)
        {
            return resource.Send(HttpMethod.Put, uri, new StringBodyContent(body), pipelineOptions);
        }

        public static Response Put(this Resource resource, string uri, BodyContent body, params PipelineOption[] pipelineOptions)
        {
            return resource.Send(HttpMethod.Put, uri, body, pipelineOptions);
        }

        public static Response Head(this Resource resource, string uri, params PipelineOption[] pipelineOptions)
        {
            return resource.Send(HttpMethod.Head, uri, null, pipelineOptions);
        }

        public static Response Delete(this Resource resource, string uri, params PipelineOption[] pipelineOptions)
        {
            return resource.Send(HttpMethod.Delete, uri, null, pipelineOptions);
        }
    }
}
