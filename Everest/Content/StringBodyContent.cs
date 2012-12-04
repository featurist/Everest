namespace Everest.Content
{
    public class StringBodyContent : BodyContent
    {
        private readonly string _body;

        public StringBodyContent(string body)
        {
            _body = body;
        }

        public string AsString()
        {
            return _body;
        }

        public string MediaType { get { return null;  } }
    }
}