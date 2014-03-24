using System;
using Everest.Pipeline;

namespace Everest.Timing
{
    public class RequestTimeout : PipelineOption
    {
        private readonly TimeSpan _timeSpan;

        public RequestTimeout(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
        }
    }
}
