using System.IO;

namespace Everest.Content
{
    public class StreamBodyContent : BodyContent
    {
        private readonly Stream _stream;
        private readonly string _mediaType;

        public StreamBodyContent(Stream stream, string mediaType)
        {
            _stream = stream;
            _mediaType = mediaType;
        }

        public Stream AsStream()
        {
            return _stream;
        }

        public string MediaType { get { return _mediaType;  } }
    }
}