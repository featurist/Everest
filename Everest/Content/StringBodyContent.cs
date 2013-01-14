using System.IO;
using System.Text;

namespace Everest.Content
{
    public class StringBodyContent : BodyContent
    {
        private readonly string _body;

        public StringBodyContent(string body)
        {
            _body = body;
        }

        public Stream AsStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_body));
        }

        public string MediaType { get { return null;  } }
    }
}