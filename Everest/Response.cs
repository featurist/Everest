using System;
using System.Net;

namespace Everest
{
    public interface Response : Resource
    {
        string Body { get; }
        byte[] BodyAsByteArray { get; }
        string ContentType { get; }
        HttpStatusCode StatusCode { get; }
        DateTimeOffset? LastModified { get; }
        string Location { get; }
    }
}