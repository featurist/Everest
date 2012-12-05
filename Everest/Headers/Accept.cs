using System.Collections.Generic;

namespace Everest.Headers
{
    public class Accept : SetRequestHeaders
    {
        public Accept(string accept) : base(new Dictionary<string, string> { { "Accept", accept } })
        {
        }
    }
}
