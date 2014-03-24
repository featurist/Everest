using Everest.Caching;
using Everest.Compression;
using Everest.Redirection;
using Everest.Timing;

namespace Everest.SystemNetHttp
{
    public struct AdapterOptions
    {
        public AutoRedirect AutoRedirect;
        public CachePolicy CachePolicy;
        public AcceptEncoding AcceptEncoding;
        public RequestTimeout Timeout;
    }
}