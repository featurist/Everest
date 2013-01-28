using System.Net.Http;
using Everest.Pipeline;
using Everest.Redirection;

namespace Everest.SystemNetHttp {
    public class SystemNetHttpClientAdapterFactory : HttpClientAdapterFactory
    {
        public HttpClientAdapter CreateClient(PipelineOptions options) {
            AutoRedirect redirectOption = null;
            options.Use<AutoRedirect>(option => {
                redirectOption = option;
            });
            return new SystemNetHttpClientAdapter(redirectOption);
        }
    }
}