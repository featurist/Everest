using System.Collections;
using System.Collections.Generic;
using Everest.Pipeline;

namespace Everest.Headers
{
    public class SetRequestHeaders : PipelineOption, IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();

        public SetRequestHeaders(IDictionary<string, string> headers)
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