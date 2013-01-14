namespace Everest.Content
{
    public class JsonBodyContent : StringBodyContent
    {
        public JsonBodyContent(string json)
            : base(json, "application/json")
        {
        }
    }
}