using Everest.Pipeline;

namespace Everest.SystemNetHttp
{
    public struct CachePolicy : PipelineOption
    {
        public bool Cache { get; set; }
    }
}