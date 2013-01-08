using System;
using System.Linq;
using System.Net.Http;
using Everest.Pipeline;

namespace Everest.Headers
{
    public class EnsureAcceptableResponseHeaders : PipelineElement
    {
        public void ApplyToRequest(HttpRequestMessage request, PipelineOptions pipelineOptions)
        {
            // Do nothing
        }

        public void ApplyToResponse(HttpResponseMessage response, PipelineOptions pipelineOptions)
        {
            pipelineOptions.Use<ExpectResponseHeaders>(option => AssertResponseHeadersAreExpected(option, response));
        }

        private void AssertResponseHeadersAreExpected(ExpectResponseHeaders expectResponseHeaders, HttpResponseMessage response)
        {
            foreach (var header in expectResponseHeaders)
            {
                if (!response.Headers.Contains(header.Key))
                {
                    throw new UnexpectedResponseHeaderException(header.Key, header.Value);
                }

                string actualValue = response.Headers.First(h => h.Key == header.Key).Value.FirstOrDefault();
                if (actualValue != header.Value)
                {
                    throw new UnexpectedResponseHeaderException(header.Key, header.Value, actualValue);
                }
            }
        }
    }
}