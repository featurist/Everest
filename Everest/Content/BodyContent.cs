using System.IO;

namespace Everest.Content
{
    public interface BodyContent
    {
        Stream AsStream();
        string MediaType { get; }
    }
}