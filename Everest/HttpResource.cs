using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using Everest.Pipeline;

namespace Everest
{
    public class HttpResource : RestClient
    {
        private readonly HttpResponseMessage _httpResponseMessage;

        public HttpResource(HttpResponseMessage httpResponseMessage, Uri url, IEnumerable<PipelineOption> ambientPipelineOptions) : base(url, ambientPipelineOptions)
        {
            _httpResponseMessage = httpResponseMessage;
        }

        public string Body
        {
            get { return _httpResponseMessage.Content.ReadAsStringAsync().Result; }
        }

        public byte[] BodyAsByteArray
        {
            get { return _httpResponseMessage.Content.ReadAsByteArrayAsync().Result; }
        }

        public XDocument BodyAsXml
        {
            get { return XDocument.Parse(Body); }
        }

        public string ContentType
        {
            get
            {
                return _httpResponseMessage.Content.Headers.ContentType == null ? null : _httpResponseMessage.Content.Headers.ContentType.MediaType;
            }
        }

        public HttpStatusCode StatusCode
        {
            get { return _httpResponseMessage.StatusCode; }
        }

        public DateTimeOffset? LastModified
        {
            get { return _httpResponseMessage.Content.Headers.LastModified; }
        }

        public string Location
        {
            get
            {
                var location = _httpResponseMessage.Headers.Location;
                if (location != null)
                {
                    return location.ToString();
                }
                throw new KeyNotFoundException("There was no location header in response from " + Url);
            }
        }
    }
}