using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Everest
{
    public interface Response : Resource, IDisposable
    {
        string Body { get; }
        byte[] BodyAsByteArray { get; }
        Stream BodyAsStream { get; }
        string ContentType { get; }
        HttpStatusCode StatusCode { get; }
        DateTimeOffset? LastModified { get; }
        string Location { get; }
        IDictionary<string, string> Headers { get; }
    }
}