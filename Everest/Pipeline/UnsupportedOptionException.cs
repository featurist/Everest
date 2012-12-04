using System;

namespace Everest.Pipeline
{
    public class UnsupportedOptionException : Exception
    {
        private readonly PipelineOption _option;

        public UnsupportedOptionException(PipelineOption option)
        {
            _option = option;
        }

        public override string Message
        {
            get { return string.Format("Option {0} is not supported by the pipeline", _option); }
        }
    }
}