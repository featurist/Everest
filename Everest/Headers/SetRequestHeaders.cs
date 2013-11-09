using System;
using System.Collections;
using System.Collections.Generic;
using Everest.Pipeline;

namespace Everest.Headers
{
    public class ComputeRequestHeaders : PipelineOption, IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Func<IDictionary<string, string>> _headers = () => new Dictionary<string, string>();

        public ComputeRequestHeaders(Func<IDictionary<string, string>> headers)
        {
            _headers = headers;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _headers().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class SetRequestHeaders : PipelineOption, IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _headers = new Dictionary<string, string>();

        public SetRequestHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            _headers = headers;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _headers.GetEnumerator();
        }
    }
}