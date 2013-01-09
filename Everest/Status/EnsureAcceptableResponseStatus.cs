using System.Net.Http;
using Everest.Pipeline;

namespace Everest.Status
{
    internal class EnsureAcceptableResponseStatus : PipelineElement
    {
        public void ApplyToRequest(HttpRequestMessage request, PipelineOptions pipelineOptions)
        {
        }

        public void ApplyToResponse(HttpResponseMessage response, PipelineOptions pipelineOptions)
        {
            StatusAcceptability acceptable = new ExpectStatusNotInRange(400, 599);
            pipelineOptions.Use<StatusAcceptability>(option => { acceptable = option; });
            AssertResponseHasAcceptableStatus(response, acceptable);
        }

        private void AssertResponseHasAcceptableStatus(HttpResponseMessage response, StatusAcceptability acceptableStatuses)
        {
            if (!acceptableStatuses.IsStatusAcceptable(response.StatusCode))
            {
                throw new UnexpectedStatusException((int)response.StatusCode,
                    string.Format(
                    "{0} {1} -- expected response status to be {2}, got {3} ({4})\n\n\n{5}",
                                response.RequestMessage.Method,
                                response.RequestMessage.RequestUri.AbsoluteUri,
                                acceptableStatuses.DescribeAcceptableStatuses(),
                                (int)response.StatusCode,
                                response.StatusCode,
                                response.Content.ReadAsStringAsync().Result));
            }
        }
    }
}