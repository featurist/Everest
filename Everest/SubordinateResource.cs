using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Everest.Headers;
using Everest.Pipeline;

namespace Everest
{
    internal class SubordinateResource : RestClient, Response
    {
        private readonly HttpResponseMessage _httpResponseMessage;
        private IDictionary<string, string> _headers;

        public SubordinateResource(HttpResponseMessage httpResponseMessage, Uri url, HttpClientAdapter adapter, IEnumerable<PipelineOption> ambientPipelineOptions)
            : base(url, adapter, ambientPipelineOptions)
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

        public string ContentType
        {
            get { return _httpResponseMessage.Content.Headers.ContentType == null ? null : _httpResponseMessage.Content.Headers.ContentType.MediaType; }
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

        public IDictionary<string, string> Headers
        {
            get { return _headers ?? (_headers = _httpResponseMessage.AllHeadersAsStrings()); }
        }
    }
}