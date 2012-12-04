using System;
using System.Net;
using System.Xml.Linq;

namespace Everest
{
    public interface Response : Resource
    {
        string Body { get; }
        byte[] BodyAsByteArray { get; }
        XDocument BodyAsXml { get; }
        string ContentType { get; }
        HttpStatusCode StatusCode { get; }
        DateTimeOffset? LastModified { get; }
        string Location { get; }
    }
}