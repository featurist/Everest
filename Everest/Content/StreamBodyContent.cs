using System.Collections.Generic;
using System.IO;

namespace Everest.Content
{
    public class StreamBodyContent : BodyContent
    {
        private readonly Stream _stream;
        private readonly string _mediaType;
        private readonly Dictionary<string, string> _headers;

        public StreamBodyContent(Stream stream, string mediaType)
        {
            _stream = stream;
            _mediaType = mediaType;
            _headers = new Dictionary<string, string>  { {"Content-Type", _mediaType} };
        }

        public Stream AsStream()
        {
            return _stream;
        }

        public string MediaType { get { return _mediaType;  } }

        public IDictionary<string, string> Headers { get { return _headers; } }
    }
}