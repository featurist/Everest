using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Everest.Auth;
using Everest.Content;
using Everest.Headers;
using Everest.Pipeline;
using Everest.Status;
using Everest.SystemNetHttp;

namespace Everest
{
    public class RestClient : Resource
    {
        private readonly List<PipelineElement> _pipeline = new List<PipelineElement>
                                                      {
                                                          new AddCustomRequestHeaders(),
                                                          new AddBasicAuthHeaders(),
                                                          new EnsureAcceptableResponseStatus(),
                                                          new EnsureAcceptableResponseHeaders()
                                                      };

        private static readonly PipelineOption[] DefaultAmbientOptions = new PipelineOption[] {};
        private static readonly HttpClientAdapter DefaultAdapter = new SystemNetHttpClientAdapter();

        private readonly IEnumerable<PipelineOption> _ambientPipelineOptions;
        protected readonly HttpClientAdapter Adapter;

        public RestClient() : this(DefaultAmbientOptions)
        {
        }

        public RestClient(params PipelineOption[] options) : this(null, options)
        {
        }

        public RestClient(string url, params PipelineOption[] options) : this(new Uri(url), DefaultAdapter, options)
        {
        }

        public RestClient(string url, HttpClientAdapter adapter, IEnumerable<PipelineOption> ambientPipelineOptions) : this(new Uri(url), adapter, ambientPipelineOptions)
        {
        }

        public RestClient(Uri url, HttpClientAdapter adapter, IEnumerable<PipelineOption> ambientPipelineOptions)
        {
            Url = url;
            Adapter = adapter;
            _ambientPipelineOptions = ambientPipelineOptions;
        }

        public event EventHandler<RequestEventArgs> Sending;
        public event EventHandler<ResponseEventArgs> Responded;
        public event EventHandler<RequestErrorEventArgs> SendError;

        public Uri Url { get; private set; }

        public static string ResolveTemplate(string uri, object arguments)
        {
            return arguments.GetType().GetProperties()
                .Aggregate(uri, (resolved, prop) => resolved.Replace("{" + prop.Name + "}", prop.GetValue(arguments, null).ToString()));
        }

        public Response Send(HttpMethod method, string uri, BodyContent body, params PipelineOption[] overridingPipelineOptions)
        {
            var options = new PipelineOptions(_ambientPipelineOptions.Concat(overridingPipelineOptions));
            HttpRequestMessageRequestDetails requestDetails;
            var request = CreateRequestMessage(method, uri, body, options, out requestDetails);
            var response = TrySending(request, requestDetails, options);
            return new SubordinateResource(response, response.RequestMessage.RequestUri, Adapter, _ambientPipelineOptions);
        }

        private HttpResponseMessage TrySending(HttpRequestMessage request, HttpRequestMessageRequestDetails requestDetails, PipelineOptions options)
        {
            HttpResponseMessage response;
            try
            {
                response = Adapter.SendAsync(request).Result;
            }
            catch (AggregateException exception)
            {
                if (SendError != null)
                {
                    requestDetails = requestDetails ?? new HttpRequestMessageRequestDetails(request);
                    SendError(this, new RequestErrorEventArgs(requestDetails, exception.InnerException));
                }
                throw exception.InnerException;
            }

            if (Responded != null)
            {
                requestDetails = requestDetails ?? new HttpRequestMessageRequestDetails(request);
                Responded(this, new ResponseEventArgs(requestDetails, new HttpResponseMessageResponseDetails(requestDetails, response)));
            }
            ApplyPipelineToResponse(response, options);
            options.AssertAllOptionsWereUsed();
            return response;
        }

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string uri, BodyContent body, PipelineOptions options,
                                                        out HttpRequestMessageRequestDetails requestDetails)
        {
            var absoluteUri = AbsoluteUrlOf(uri);
            var request = new HttpRequestMessage();
            ApplyPipelineToRequest(request, options);
            if (body != null)
            {
                var content = new StreamContent(body.AsStream());
                if (body.MediaType != null)
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(body.MediaType);
                }
                request.Content = content;
            }
            request.RequestUri = absoluteUri;
            request.Method = method;
            requestDetails = null;
            if (Sending != null)
            {
                requestDetails = new HttpRequestMessageRequestDetails(request);
                Sending(this, new RequestEventArgs(requestDetails));
            }
            return request;
        }

        public Resource With(params PipelineOption[] options)
        {
            return new RestClient(Url, Adapter, _ambientPipelineOptions.Concat(options));
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
