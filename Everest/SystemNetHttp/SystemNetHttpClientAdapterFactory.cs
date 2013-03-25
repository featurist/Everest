using System.Collections.Generic;
using Everest.Pipeline;
using Everest.Redirection;

namespace Everest.SystemNetHttp
{
    public class SystemNetHttpClientAdapterFactory : HttpClientAdapterFactory
    {
        private static readonly Dictionary<AutoRedirect, HttpClientAdapter> Adapters =
            new Dictionary<AutoRedirect, HttpClientAdapter> {

                { AutoRedirect.DoNotAutoRedirect,
                    new SystemNetHttpClientAdapter(AutoRedirect.DoNotAutoRedirect) },

                { AutoRedirect.AutoRedirectAndForwardAuthorizationHeader,
                    new SystemNetHttpClientAdapter(AutoRedirect.AutoRedirectAndForwardAuthorizationHeader) },

                { AutoRedirect.AutoRedirectButDoNotForwardAuthorizationHeader,
                    new SystemNetHttpClientAdapter(AutoRedirect.AutoRedirectButDoNotForwardAuthorizationHeader) }
            
            };

        public HttpClientAdapter CreateClient(PipelineOptions options)
        {
            var redirectOption = AutoRedirect.AutoRedirectButDoNotForwardAuthorizationHeader;
            options.Use<AutoRedirect>(option => { redirectOption = option; });
            return Adapters[redirectOption];
        }
    }
}