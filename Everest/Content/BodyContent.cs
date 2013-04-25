using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Everest.Content
{
    public interface BodyContent
    {
        Stream AsStream();
        string MediaType { get; }
        IDictionary<string, string> Headers { get; }
    }
}