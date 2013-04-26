using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everest.Content
{
    public class WwwFormUrlEncodedContent : StringBodyContent
    {
        public WwwFormUrlEncodedContent(object values, string mediaType = "application/x-www-form-urlencoded")
            : base(BuildBody(values), mediaType)
        {
        }

        public WwwFormUrlEncodedContent(Dictionary<string, string> values, string mediaType = "application/x-www-form-urlencoded")
            : base(BuildBody(values), mediaType)
        {
        }

        private static string BuildBody(object values)
        {
            var properties = values.GetType().GetProperties();

            return BuildBody(properties
                .Where(x => x.GetIndexParameters().Length == 0)
                .ToDictionary(x => x.Name, x=>x.GetValue(values, null).ToString()));
        }

        private static string BuildBody(Dictionary<string, string> values)
        {
            return String.Join("&", values.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value)));
        }
    }
}
