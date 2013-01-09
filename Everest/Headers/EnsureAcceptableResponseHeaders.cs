using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Everest.Pipeline;

namespace Everest.Headers
{
    public class EnsureAcceptableResponseHeaders : PipelineElement
    {
        public void ApplyToRequest(HttpRequestMessage request, PipelineOptions pipelineOptions)
        {
        }

        public void ApplyToResponse(HttpResponseMessage response, PipelineOptions pipelineOptions)
        {
            pipelineOptions.Use<ExpectResponseHeaders>(option => AssertResponseHeadersAreExpected(option, response.AllHeadersAsStrings()));
        }

        private static void AssertResponseHeadersAreExpected(IEnumerable<KeyValuePair<string, string>> expectedResponseHeaders, IDictionary<string, string> actualResponseHeaders)
        {
            foreach (var expectedHeader in expectedResponseHeaders)
            {
                if (!actualResponseHeaders.ContainsKey(expectedHeader.Key))
                {
                    throw new UnexpectedResponseHeaderException(expectedHeader.Key, expectedHeader.Value);
                }

                var actualValue = actualResponseHeaders.First(h => h.Key == expectedHeader.Key).Value;
                if (actualValue != expectedHeader.Value)
                {
                    throw new UnexpectedResponseHeaderException(expectedHeader.Key, expectedHeader.Value, actualValue);
                }
            }
        }
    }
}