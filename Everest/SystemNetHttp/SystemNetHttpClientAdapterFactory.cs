using System.Collections.Generic;
using Everest.Pipeline;
using Everest.Redirection;

namespace Everest.SystemNetHttp
{
    public class SystemNetHttpClientAdapterFactory : HttpClientAdapterFactory
    {
        private static readonly Dictionary<AdapterOptions, HttpClientAdapter> Adapters =
            new Dictionary<AdapterOptions, HttpClientAdapter>();

        public HttpClientAdapter CreateClient(PipelineOptions options)
        {
            var adapterOptions = new AdapterOptions
            {
                AutoRedirect = AutoRedirect.AutoRedirectButDoNotForwardAuthorizationHeader,
                CachePolicy = new CachePolicy {Cache = false}
            };

            options.Use<AutoRedirect>(option => { adapterOptions.AutoRedirect = option; });
            options.Use<CachePolicy>(option => { adapterOptions.CachePolicy = option; });

            return CreateClient(adapterOptions);
        }

        private HttpClientAdapter CreateClient(AdapterOptions options)
        {
            HttpClientAdapter adapter;
            if (!Adapters.TryGetValue(options, out adapter))
            {
                Adapters[options] = adapter = new SystemNetHttpClientAdapter(options);
            }
            return adapter;
        }
    }

    public struct AdapterOptions
    {
        public AutoRedirect AutoRedirect;
        public CachePolicy CachePolicy;
    }
}