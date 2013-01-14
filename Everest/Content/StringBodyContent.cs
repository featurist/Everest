using System.IO;
using System.Text;

namespace Everest.Content
{
    public class StringBodyContent : BodyContent
    {
        private readonly string _body;
        private readonly string _mediaType;

        public StringBodyContent(string body, string mediaType = null)
        {
            _body = body;
            _mediaType = mediaType;
        }

        public StringBodyContent(string body)
        {
            _body = body;
        }

        public Stream AsStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_body));
        }

        public string MediaType { get { return _mediaType;  } }
    }
}