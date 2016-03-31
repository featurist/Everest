using Everest.Caching;
using Everest.Compression;
using Everest.Redirection;
using Everest.Timing;
using Everest.Proxy;

namespace Everest.SystemNetHttp
{
    public struct AdapterOptions
    {
        public AutoRedirect AutoRedirect;
        public CachePolicy CachePolicy;
        public AcceptEncoding AcceptEncoding;
        public RequestTimeout Timeout;
        public WebProxy WebProxy;
    }
}
