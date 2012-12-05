using System.Xml.Linq;

namespace Everest
{
    public static class Extensions
    {
        public static XDocument BodyAsXml(this Response response)
        {
            return XDocument.Parse(response.Body);
        }
    }
}
