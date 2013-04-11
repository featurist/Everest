using Everest.Pipeline;

namespace Everest.Compression
{
    public struct AcceptEncoding : PipelineOption
    {
        public bool AcceptGzipAndDeflate;
    }
}