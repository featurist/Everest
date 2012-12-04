using System;
using System.Net.Http;
using System.Text;
using Everest.Pipeline;

namespace Everest.Auth
{
    public class AddBasicAuthHeaders : PipelineElement
    {
        public void ApplyToRequest(HttpRequestMessage request, PipelineOptions pipelineOptions)
        {
            pipelineOptions.Use<BasicAuth>(auth => AddAuthorizationHeader(request, auth.Username, auth.Password));
        }

        private static void AddAuthorizationHeader(HttpRequestMessage request, string username, string password)
        {
            var usernamePasswordPair = Encoding.ASCII.GetBytes(username + ":" + password);
            var encoded = Convert.ToBase64String(usernamePasswordPair);
            request.Headers.Add("Authorization", "Basic " + encoded);
        }

        public void ApplyToResponse(HttpResponseMessage response, PipelineOptions pipelineOptions)
        {
        }
    }
}