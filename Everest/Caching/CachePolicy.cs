using Everest.Pipeline;

namespace Everest.Caching
{
    public struct CachePolicy : PipelineOption
    {
        public bool Cache { get; set; }
    }
}