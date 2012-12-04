using System;
using System.Collections.Generic;
using Everest.Headers;

namespace Everest.Caching
{
    public class IfModifiedSince : SetRequestHeaders
    {
        public IfModifiedSince(DateTimeOffset dateTime) : base(new Dictionary<string, string> { { "If-Modified-Since", dateTime.ToString("r") } })
        {
        }
    }
}