namespace Everest.Content
{
    public class JsonBodyContent : BodyContent
    {
        private readonly string _json;

        public JsonBodyContent(string json)
        {
            _json = json;
        }

        public string AsString()
        {
            return _json;
        }

        public string MediaType { get { return "application/json"; } }
    }
}