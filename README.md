# Everest

A resource-oriented HTTP client for .net

### Install

via [NuGet](http://nuget.org):

    PM> Install-Package Everest

### Usage

    var client = new Everest.RestClient();
    
    // GET http://www.google.com
    var homePage = client.Get("http://www.google.com");
    
    // GET http://www.google.com/search?q=everest+http+client
    var searchResults = homePage.Get("search?q=everest+http+client");
    
    ...

#### Resource-Oriented?

To use Everest, create a [RestClient](Everest/RestClient.cs):

    var client = new Everest.RestClient();

A [RestClient](Everest/RestClient.cs) is a [Resource](Everest/Resource.cs), therefore it can be used to make requests, such as "Get" requests:

    var homePage = client.Get("http://www.google.com");

The response (homePage) is a [Response](Everest/Response.cs), which itself is also a [Resource](Everest/Resource.cs), so we can call "Get" on that too, to get a new Response, relative to the URI that responded to the first request:

    // GET http://www.google.com/search?q=everest+http+client
    var searchResults = homePage.Get("search?q=everest+http+client");

All requests return Responses, so we can do this over and over.

#### Resource API

Everest requests are ultimately dispatched by calling the "Send" method on a [Resource](Everest/Resource.cs), to get a [Response](Everest/Response.cs):

    namespace Everest
    {
        public interface Resource
        {
            ...
            Response Send(HttpMethod method, string uri, BodyContent body, params PipelineOption[] overridingPipelineOptions);
            ...
        }
    }

Most of the time, you have a particular verb in mind when you make an HTTP request, so you'll want the convenient [Resource API extension methods](Everest/ResourceApi.cs):

    namespace Everest
    {
        public static class ResourceApi
        {
            ...
            public static Response Get(this Resource resource, string uri, params PipelineOption[] pipelineOptions)
            {
                return resource.Send(HttpMethod.Get, uri, null, pipelineOptions);
            }
            ...
        }
    }

#### Pipelines

When you make HTTP requests, you often need to set specific parameters (such as request headers), or handle responses in a particular way. For anything beyond the request URI and body, Everest allows the request and/or response handling to be tweaked, using [pipeline options](Everest/Pipeline/PipelineOption.cs):

    using Everest;
    using Everest.Headers;
    
    ...
    
    var client = new RestClient(new RequestHeader("X-Requested-With", "Everest"));

[RequestHeader](Everest/Headers/RequestHeader.cs) is one example of a [PipelineOption](Everest/Pipeline/PipelineOption.cs), which are used to customise the way Everest handles requests and responses.

#### Pipeline Options

[PipelineOption](Everest/Pipeline/PipelineOption.cs) is a [marker interface](http://en.wikipedia.org/wiki/Marker_interface_pattern) implemented by anything that can customise a request or its response handling.

Pipeline options can be applied to _all_ requests by passing them to the RestClient constructor:
    
    var options = new PipelineOption[] { ExpectStatus.OK, new RequestHeader("foo", "bar") }
    
    new RestClient(options);

...or applied to a single request at a time (often _overriding_ existing options):

    new RestClient().Get(url, new RequestHeader("foo", "baz"))

The following pipeline options can be used to customise request/response behaviour:

* [BasicAuth](Everest/Auth/BasicAuth.cs) sets the '[Authorization](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.8)' header for [HTTP Basic authentication](http://en.wikipedia.org/wiki/Basic_access_authentication)
    
* [IfModifiedSince](Everest/Caching/IfModifiedSince.cs) sets the '[If-Modified-Since](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.25)' HTTP header
    
* [Accept](Everest/Headers/Accept.cs) sets the '[Accept](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.1)' HTTP header
    
* [SetRequestHeaders](Everest/Headers/SetRequestHeaders.cs) sets arbitrary HTTP request headers.
    
* [ExpectResponseHeaders](Everest/Headers/ExpectResponseHeaders.cs) throws exceptions when unexpected header values are present.
    
* [ExpectStatus](Everest/Status/ExpectStatus.cs) throws exceptions when the response status code is unexpected.
    
* [ExpectStatusInRange](Everest/Status/ExpectStatusInRange.cs) throws exceptions when the response status code is outside a range of values.
    
* [ExpectStatusNotInRange](Everest/Status/ExpectStatusNotInRange.cs) throws exceptions when the response status code is inside a range of values.

#### Body Content

An implementation of [BodyContent](Everest/Content/BodyContent) can be passed into any request that takes a payload (e.g. POST/PUT):

    namespace Everest.Content
    {
        public interface BodyContent
        {
            Stream AsStream();
            string MediaType { get; }
        }
    }

A couple of implementations of BodyContent should come in handy:

* Use [StringBodyContent](Everest/Content/StringBodyContent.cs) to send plain old text/plain
    
* [StreamBodyContent](Everest/Content/StreamBodyContent.cs) for stuff that can be streamed

* [JsonBodyContent](Everest/Content/JsonBodyContent.cs) for a JSON request body

#### Status Codes

By default, Everest throws an exception if the response status code is in the range 400-599.

You can override the acceptable response status code:
    
    // Create a client that doesn't usually complain about errors
    client = new RestClient(ExpectStatus.IgnoreStatus)
    
    // GET request, expecting a 404
    client.Get("foo", ExpectStatus.NotFound)

    // POST request, expecting a 201
    client.Post("foo", "body", new ExpectStatus(HttpStatusCode.Created))

#### Redirection

By default, Everest will automatically follow redirects. Change that behaviour with the [AutoRedirect](Everest/Redirection/AutoRedirect.cs) option:

    client.Post("/foos", "body", AutoRedirect.DoNotAutoRedirect)

For security, Authorization headers are not sent in requests following automatic redirects. This can be overridden:

    client.Put("/foos/1", "body", AutoRedirect.AutoRedirectAndForwardAuthorizationHeader)

#### Builder API

Given a resource, Resource.With(params[] options) returns a new client with overridden default options:

    client = new RestClient();
    authenticated client = client.With(new BasicAuth("user", "pass"));
    ajax client = client.With(new RequestHeader("X-Requested-With", "ajax"));

#### Everest make conservatories in the UK

I know, sorry. I work with a guy called [Julian Everett](http://julianeverett.wordpress.com), who is an expert in the art of [REST](http://en.wikipedia.org/wiki/Representational_state_transfer). This project is named (nearly) after him.

