using System.Collections.Concurrent;
using System.Collections.Generic;
using Everest.Caching;
using Everest.Compression;
using Everest.Pipeline;
using Everest.Redirection;

namespace Everest.SystemNetHttp
{
    public class SystemNetHttpClientAdapterFactory : HttpClientAdapterFactory
    {
        private static readonly ConcurrentDictionary<AdapterOptions, HttpClientAdapter> Adapters =
            new ConcurrentDictionary<AdapterOptions, HttpClientAdapter>();

        public HttpClientAdapter CreateClient(PipelineOptions options)
        {
            var adapterOptions = new AdapterOptions
            {
                AutoRedirect = AutoRedirect.AutoRedirectButDoNotForwardAuthorizationHeader,
                CachePolicy = new CachePolicy {Cache = false},
                AcceptEncoding = new AcceptEncoding {AcceptGzipAndDeflate = true}
            };

            options.Use<AutoRedirect>(option => { adapterOptions.AutoRedirect = option; });
            options.Use<CachePolicy>(option => { adapterOptions.CachePolicy = option; });
            options.Use<AcceptEncoding>(option => { adapterOptions.AcceptEncoding = option; });

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
}