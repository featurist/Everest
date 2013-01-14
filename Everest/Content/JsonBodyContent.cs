namespace Everest.Content
{
    public class JsonBodyContent : StringBodyContent
    {
        public JsonBodyContent(string json) : base(json)
        {
        }

        public string MediaType { get { return "application/json"; } }
    }
}