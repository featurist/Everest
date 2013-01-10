using System.Collections.Generic;

namespace Everest.Headers
{
    public class RequestHeader : SetRequestHeaders
    {
        public RequestHeader(string key, string value) : base(new Dictionary<string, string> { { key, value } })
        {
        }
    }
}
