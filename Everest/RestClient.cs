using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Everest.Auth;
using Everest.Content;
using Everest.Headers;
using Everest.Pipeline;
using Everest.Status;

namespace Everest
{
    public class RestClient
    {
        private readonly HttpClient _client = new HttpClient();
        
        private readonly List<PipelineElement> _pipeline = new List<PipelineElement>
                                                      {
                                                          new AddCustomRequestHeaders(),
                                                          new AddBasicAuthHeaders(),
                                                          new EnsureAcceptableResponseStatus()
                                                      };

        private static readonly PipelineOption[] DefaultOptions = new PipelineOption[]
                                                                       {
                                                                       };

        private readonly IEnumerable<PipelineOption> _ambientPipelineOptions;

        public RestClient() : this(null, DefaultOptions)
        {
        }

        public RestClient(params PipelineOption[] options) : this(null, options)
        {
        }

        protected RestClient(Uri resourceUri, IEnumerable<PipelineOption> ambientPipelineOptions)
        {
            _ambientPipelineOptions = ambientPipelineOptions;
            Url = resourceUri;
            _client.DefaultRequestHeaders.Add("Accept", "*/*");
        }

        public Uri Url { get; private set; }

        public static string ResolveTemplate(string uri, object arguments)
        {
            return arguments.GetType().GetProperties()
                .Aggregate(uri, (resolved, prop) => resolved.Replace("{" + prop.Name + "}", prop.GetValue(arguments, null).ToString()));
        }

        public HttpResource Get(string uri, params PipelineOption[] pipelineOptions)
        {
            return Send(HttpMethod.Get, uri, null, pipelineOptions);
        }

        public HttpResource Get(string uri, IDictionary<string, string> headers, params PipelineOption[] options)
        {
            return Get(uri, options.Union(new[] {new SetRequestHeaders(headers)}).ToArray());
        }

        public HttpResource Post(string url, string body, params PipelineOption[] pipelineOptions)
        {
            return Send(HttpMethod.Post, url, new StringBodyContent(body), pipelineOptions);
        }

        public HttpResource Put(string uri, string body, params PipelineOption[] pipelineOptions)
        {
            return Send(HttpMethod.Put, uri, new StringBodyContent(body), pipelineOptions);
        }

        public HttpResource Put(string uri, BodyContent body, params PipelineOption[] pipelineOptions)
        {
            return Send(HttpMethod.Put, uri, body, pipelineOptions);
        }

        public HttpResource Head(string uri, params PipelineOption[] pipelineOptions)
        {
            return Send(HttpMethod.Head, uri, null, pipelineOptions);
        }

        public HttpResource Delete(string uri, params PipelineOption[] pipelineOptions)
        {
            return Send(HttpMethod.Delete, uri, null, pipelineOptions);
        }

        private HttpResource Send(HttpMethod method, string uri, BodyContent body, params PipelineOption[] overridingPipelineOptions)
        {
            var options = new PipelineOptions(_ambientPipelineOptions.Concat(overridingPipelineOptions));

            var absoluteUri = AbsoluteUrlOf(uri);
            var request = new HttpRequestMessage();
            ApplyPipelineToRequest(request, options);

            if (body != null)
            {
                request.Content = new StringContent(body.AsString(), Encoding.UTF8, body.MediaType);
            }
            request.RequestUri = absoluteUri;
            request.Method = method;
            var response = _client.SendAsync(request).Result;
            ApplyPipelineToResponse(response, options);

            options.AssertAllOptionsWereUsed();

            return new HttpResource(response, response.RequestMessage.RequestUri, _ambientPipelineOptions);
        }

        private void ApplyPipelineToRequest(HttpRequestMessage request, PipelineOptions options)
        {
            foreach (var element in _pipeline)
            {
                element.ApplyToRequest(request, options);
            }
        }

        private void ApplyPipelineToResponse(HttpResponseMessage response, PipelineOptions options)
        {
            foreach (var element in _pipeline)
            {
                element.ApplyToResponse(response, options);
            }
        }

        private Uri AbsoluteUrlOf(string relativeOrAbsoluteUrl)
        {
            if (Url != null)
            {
                return new Uri(Url, relativeOrAbsoluteUrl);
            }
            if (Uri.IsWellFormedUriString(relativeOrAbsoluteUrl, UriKind.Absolute))
            {
                return new Uri(relativeOrAbsoluteUrl);
            }
            throw new ApplicationException("expected an absolute URI, instead found: " + relativeOrAbsoluteUrl);
        }
    }
}
