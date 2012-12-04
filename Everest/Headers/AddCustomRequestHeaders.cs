using System.Collections.Generic;
using System.Net.Http;
using Everest.Pipeline;

namespace Everest.Headers
{
    internal class AddCustomRequestHeaders : PipelineElement
    {
        public void ApplyToRequest(HttpRequestMessage request, PipelineOptions pipelineOptions)
        {
            pipelineOptions.Use<SetRequestHeaders>(headers => AddRequestHeaders(request, headers));
        }

        private void AddRequestHeaders(HttpRequestMessage request, IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        public void ApplyToResponse(HttpResponseMessage response, PipelineOptions pipelineOptions)
        {
        }
    }
}