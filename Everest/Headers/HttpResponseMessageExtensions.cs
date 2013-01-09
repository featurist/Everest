using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Everest.Headers
{
    public static class HttpResponseMessageExtensions
    {
        public static IDictionary<string, string> AllHeadersAsStrings(this HttpResponseMessage response)
        {
            return response.Headers.Union(response.Content.Headers).ToDictionary(header => header.Key, header => String.Join(", ", header.Value));
        }
    }
}
