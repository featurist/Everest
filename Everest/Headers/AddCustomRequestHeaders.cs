using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Everest.Pipeline;

namespace Everest.Headers
{
    internal class AddCustomRequestHeaders : PipelineElement
    {
        public void ApplyToRequest(HttpRequestMessage request, PipelineOptions pipelineOptions)
        {
            pipelineOptions.UseAll<SetRequestHeaders>(options => AddRequestHeaders(request, options));
        }

        private static void AddRequestHeaders(HttpRequestMessage request, IEnumerable<SetRequestHeaders> options)
        {
            var mergedHeaders = new Dictionary<string, string>();
            foreach (var header in options.SelectMany(option => option))
            {
                mergedHeaders[header.Key] = header.Value;
            }
            foreach (var header in mergedHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        public void ApplyToResponse(HttpResponseMessage response, PipelineOptions pipelineOptions)
        {
        }
    }
}