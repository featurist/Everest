using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Everest.Content
{
    public class StringBodyContent : BodyContent
    {
        private readonly string _body;
        private readonly string _mediaType;
        private readonly Dictionary<string, string> _headers;

        public StringBodyContent(string body, string mediaType = null)
        {
            _body = body;
            _mediaType = mediaType;
            _headers = new Dictionary<string, string>();
            if (mediaType != null)
                _headers.Add("Content-Type", _mediaType);
        }
        
        public Stream AsStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_body));
        }

        public string MediaType { get { return _mediaType;  } }

        public IDictionary<string, string> Headers { get { return _headers; } }

    }
}