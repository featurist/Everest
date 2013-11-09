using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Everest.Content;
using Everest.Headers;
using Everest.Pipeline;
using Everest.Redirection;
using Everest.Status;
using NUnit.Framework;
using SelfishHttp;

namespace Everest.UnitTests
{
    [TestFixture]
    public class RestClientTest
    {
        private const string BaseAddress = "http://localhost:18745";
        private Server _server;
        private RestClient _client;

        [SetUp]
        public void StartServerAndCreateClient()
        {
            _server = new Server(18745);
            _server.OnGet("/foo").RespondWith("foo!");
            _server.OnGet("/foo/bar").RespondWith("foo bar?");
            _client = new RestClient(BaseAddress);
        }

        [TearDown]
        public void TearDown()
        {
            _server.Stop();
        }

        [Test]
        public void ReturnsNewResourceAfterEachRequest()
        {
            var fooResource = _client.Get("/foo");
            var fooBody = fooResource.Body;
            Assert.That(fooBody, Is.EqualTo("foo!"));

            var barResource = fooResource.Get("foo/bar");
            var barBody = barResource.Body;
            Assert.That(barBody, Is.EqualTo("foo bar?"));
        }

        [Test]
        public void FollowsLinksRelativeToResourceEvenAfterRedirect()
        {
            _server.OnGet("/redirect").RedirectTo("/foo/");
            _server.OnGet("/foo/").RespondWith("foo!");
            _server.OnGet("/foo/bar/baz").RespondWith("baz!");
            var body = _client.Get("/redirect").Get("bar/baz").Body;
            Assert.That(body, Is.EqualTo("baz!"));
        }

        [Test]
        public void AppliesAmbientOptionsToRedirects()
        {
            _server.OnGet("/redirect").RedirectTo("/x");
            _server.OnGet("/x").Respond((req, res) => res.Body = req.Headers["x-foo"]);
            var client = new RestClient(BaseAddress, new RequestHeader("x-foo", "yippee"));
            var body = client.Get("/redirect").Body;
            Assert.That(body, Is.EqualTo("yippee"));
        }

        [Test]
        public void AppliesAuthorizationHeaderToRedirects()
        {
            _server.OnGet("/redirect").RedirectTo("/x");
            _server.OnGet("/x").Respond((req, res) => res.Body = req.Headers["Authorization"]);
            var body = _client.Get("/redirect", new RequestHeader("Authorization", "yikes"),
                new AutoRedirect { EnableAutomaticRedirection = true, ForwardAuthorizationHeader = true }).Body;
            Assert.That(body, Is.EqualTo("yikes"));
        }

        [Test]
        public void AppliesAuthorizationHeaderToPermanentRedirects() {
            _server.OnGet("/redirect").Respond((req, res) => { res.StatusCode = (int)HttpStatusCode.MovedPermanently;
                res.Headers["Location"] = "/x";
            });
            _server.OnGet("/x").Respond((req, res) => res.Body = req.Headers["Authorization"]);
            var body = _client.Get("/redirect", new RequestHeader("Authorization", "crumbs"),
                new AutoRedirect { EnableAutomaticRedirection = true, ForwardAuthorizationHeader = true }).Body;
            Assert.That(body, Is.EqualTo("crumbs"));
        }

        [Test]
        public void AllowsAutoRedirectToBeDisabledForSingleRequest()
        {
            _server.OnGet("/redirect").RedirectTo("/x");
            var response = _client.Get("/redirect", AutoRedirect.DoNotAutoRedirect);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        }

        [Test]
        public void AppliesOverridingOptionsToRedirects() {
            _server.OnGet("/redirect").RedirectTo("/x");
            _server.OnGet("/x").Respond((req, res) => res.Body = req.Headers["x-foo"]);
            var body = _client.Get("/redirect", new RequestHeader("x-foo", "yippee")).Body;
            Assert.That(body, Is.EqualTo("yippee"));
        }

        [Test]
        public void MakesPutRequests()
        {
            _server.OnPut("/foo").RespondWith(requestBody => "putted " + requestBody);
            var body = _client.Put("/foo", "body").Body;
            Assert.That(body, Is.EqualTo("putted body"));
        }

        [Test]
        public void ExposesResponseHeaders()
        {
            _server.OnGet("/whaa").Respond((req, res) => { res.Headers["X-Custom"] = "my custom header"; });

            var response = _client.Get("/whaa", ExpectStatus.OK);
            Assert.That(response.Headers["X-Custom"], Is.EqualTo("my custom header"));
        }

        [Test]
        public void ExposesContentHeadersInTheSameCollectionAsOtherResponseHeaders()
        {
            _server.OnGet("/contentType").Respond((req, res) => { res.Headers["Content-Type"] = "x/foo"; });

            var response = _client.Get("/contentType");
            Assert.That(response.Headers.ContainsKey("Content-Type"));
            Assert.That(response.Headers["Content-Type"], Is.EqualTo("x/foo"));
        }

        [Test]
        public void MakesOptionsRequests()
        {
            _server.OnOptions("/whaa").RespondWith("options!");
            var body = _client.Options("/whaa", ExpectStatus.OK).Body;
            Assert.That(body, Is.EqualTo("options!"));
        }

        [Test]
        public void MakesPostRequests()
        {
            _server.OnPost("/foo").RespondWith(requestBody => "posted " + requestBody);
            var body = _client.Post("/foo", "body", ExpectStatus.OK).Body;
            Assert.That(body, Is.EqualTo("posted body"));
        }

        [Test]
        public void MakesPostRequestsWithBodyContent()
        {
            _server.OnPost("/foo").RespondWith(requestBody => "posted " + requestBody);
            var body = _client.Post("/foo", new StringBodyContent("body"), ExpectStatus.OK).Body;
            Assert.That(body, Is.EqualTo("posted body"));
        }

        [Test]
        public void MakesHeadRequests()
        {
            _server.OnHead("/foo").Respond((req, res) => res.StatusCode = 303);
            var response = _client.Head("/foo", new ExpectStatus(HttpStatusCode.SeeOther));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.SeeOther));
        }

        [Test]
        public void MakesPostRequestsToSelf()
        {
            _server.OnGet("/self").RespondWith("good on ya");
            _server.OnPost("/self").RespondWith(requestBody => "posted " + requestBody);
            var response = _client.Get("/self");
            var body = response.Post("body").Body;
            Assert.That(body, Is.EqualTo("posted body"));
        }

        [Test]
        public void CanPutBinaryContentInStream()
        {
            _server.OnPut("/image").Respond((req, res) =>
                {
                    var body = req.BodyAs<Stream>();
                    res.Headers["Content-Type"] = req.Headers["Content-Type"];
                    res.Body = body;
                });

            var image = _client.Put("/image", new StreamBodyContent(new MemoryStream(Encoding.UTF8.GetBytes("this is an image")), "image/png"));
            Assert.That(image.ContentType, Is.EqualTo("image/png"));
            Assert.That(image.Body, Is.EqualTo("this is an image"));
        }

        [Test]
        public void GetExpects200RangeStatusByDefault()
        {
            try
            {
                _client.Get("/non-existent");
            }
            catch (UnexpectedStatusException e)
            {
                Assert.That(e.Message, Is.EqualTo("GET http://localhost:18745/non-existent -- expected response status to be not in the range 400-599, got 404 (NotFound)\n\n\n"));
            }
        }

        [Test]
        public void GetExpectStatusIsOverridable()
        {
            Assert.That(() => _client.Get("/foo", new ExpectStatus(HttpStatusCode.InternalServerError)), Throws.InstanceOf<UnexpectedStatusException>());
        }

        [Test]
        public void PutExpectStatusIsOverridable()
        {
            Assert.That(() => _client.Put("/foo", "oops", new ExpectStatus(HttpStatusCode.InternalServerError)), Throws.InstanceOf<UnexpectedStatusException>());
        }

        [Test]
        public void ThrowsWhenUnsupportedPerRequestOptionsAreSupplied()
        {
            Assert.That(() => _client.Get("/foo", new BogusOption()), Throws.InstanceOf<UnsupportedOptionException>());
        }

        [Test]
        public void ThrowsWhenUnsupportedAmbientOptionsAreSupplied()
        {
            _server.OnGet("/blah").RespondWith("ok!");
            Assert.That(() => new RestClient(BaseAddress, new BogusOption()).Get("/blah"), Throws.InstanceOf<UnsupportedOptionException>());
        }

        [Test]
        public void AppliesPipelineOptionsToSubsequentRequests()
        {
            _server.OnGet("/headers").Respond((req, res) => res.Body = req.Headers["x-per-client"]);

            var client = new RestClient(BaseAddress, new SetRequestHeaders(new Dictionary<string, string> { { "x-per-client", "x" } }));
            var firstResponse = client.Get("/headers");
            Assert.That(firstResponse.Body, Is.EqualTo("x"));
            var secondResponse = firstResponse.Get("/headers");
            Assert.That(secondResponse.Body, Is.EqualTo("x"));
        }

        [Test]
        public void AppliesPipelineOptionsPerRequest()
        {
            _server.OnGet("/headers").Respond((req, res) => res.Body = req.Headers["x-per-client"]);

            var client = new RestClient(BaseAddress);
            var firstResponse = client.Get("/headers", new SetRequestHeaders(new Dictionary<string, string> { { "x-per-client", "x" } }));
            Assert.That(firstResponse.Body, Is.EqualTo("x"));
            var secondResponse = firstResponse.Get("/headers");
            Assert.That(secondResponse.Body, Is.EqualTo(""));
        }

        [Test]
        public void ProvidesAConvenientWayToSetAcceptHeader()
        {
            _server.OnGet("/accept").Respond((req, res) => res.Body = req.Headers["Accept"]);
            var client = new RestClient(BaseAddress);
            var response = client.Get("/accept", new Accept("foo/bar"));
            Assert.That(response.Body, Is.EqualTo("foo/bar"));
        }

        [Test]
        public void CanComputeHeadersDynamically()
        {
            var i = 0;
            var dynamicRequestHeaders = new DynamicRequestHeaders(
                () => new Dictionary<string, string> { { "X", (++i).ToString(CultureInfo.InvariantCulture) } });

            _server.OnGet("/X").Respond((req, res) => res.Body = req.Headers["X"]);
            var client = new RestClient(BaseAddress, new SetRequestHeaders(dynamicRequestHeaders));
 
            Assert.That(client.Get("/X").Body, Is.EqualTo("1"));
            Assert.That(client.Get("/X").Body, Is.EqualTo("2"));
        }

        class DynamicRequestHeaders : IEnumerable<KeyValuePair<string, string>>
        {
            private readonly Func<Dictionary<string, string>> _func;

            public DynamicRequestHeaders(Func<Dictionary<string, string>> func)
            {
                _func = func;
            }

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                return _func().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Test]
        public void ThrowsWhenExpectedResponseHeaderIsNotSet()
        {
            _server.OnGet("/respond-with-bar").RespondWith("oops, no x header!");
            var client = new RestClient(BaseAddress, new ExpectResponseHeaders { { "x", "foo" }});
            try
            {
                client.Get("/respond-with-bar");
                Assert.Fail("Expected UnexpectedResponseHeaderException");
            }
            catch (UnexpectedResponseHeaderException e)
            {
                Assert.That(e.Key, Is.EqualTo("x"));
                Assert.That(e.ExpectedValue, Is.EqualTo("foo"));
                Assert.That(e.ActualValue, Is.EqualTo(null));
                Assert.That(e.Message, Is.EqualTo("Expected response header 'x' to have the value 'foo', but it had the value ''"));
            }
        }

        [Test]
        public void ThrowsWhenExpectedResponseHeaderHasUnexpectedValue()
        {
            _server.OnGet("/respond-with-bar").Respond((req, res) => res.Headers["x"] = "bar");
            var client = new RestClient(BaseAddress, new ExpectResponseHeaders { { "x", "foo" }});
            try
            {
                client.Get("/respond-with-bar");
                Assert.Fail("Expected UnexpectedResponseHeaderException");
            }
            catch (UnexpectedResponseHeaderException e)
            {
                Assert.That(e.Key, Is.EqualTo("x"));
                Assert.That(e.ExpectedValue, Is.EqualTo("foo"));
                Assert.That(e.ActualValue, Is.EqualTo("bar"));
                Assert.That(e.Message, Is.EqualTo("Expected response header 'x' to have the value 'foo', but it had the value 'bar'"));
            }
        }

        [Test]
        public void DoesNotThrowWhenExpectedResponseContentHeadersAreSetToExpectedValues()
        {
            _server.OnGet("/respond-with-foo").Respond((req, res) => res.Headers["Content-Type"] = "x/foo");
            var client = new RestClient(BaseAddress, new ExpectResponseHeaders { { "Content-Type", "x/foo" } });
            Assert.That(() => client.Get("/respond-with-foo"), Throws.Nothing);
        }

        [Test]
        public void ThrowsWhenExpectedResponseContentHeaderIsNotSet()
        {
            _server.OnGet("/respond-with-bar").Respond((req, res) => res.Headers["Content-Type"] = null);
            var client = new RestClient(BaseAddress, new ExpectResponseHeaders { { "Content-Type", "oh/really" } });
            try
            {
                client.Get("/respond-with-bar");
                Assert.Fail("Expected UnexpectedResponseHeaderException");
            }
            catch (UnexpectedResponseHeaderException e)
            {
                Assert.That(e.Key, Is.EqualTo("Content-Type"));
                Assert.That(e.ExpectedValue, Is.EqualTo("oh/really"));
                Assert.That(e.ActualValue, Is.EqualTo(null));
                Assert.That(e.Message, Is.EqualTo("Expected response header 'Content-Type' to have the value 'oh/really', but it had the value ''"));
            }
        }

        [Test]
        public void ThrowsWhenExpectedResponseContentHeaderHasUnexpectedValue()
        {
            _server.OnGet("/respond-with-bar").Respond((req, res) => res.Headers["Content-Type"] = "x/bar");
            var client = new RestClient(BaseAddress, new ExpectResponseHeaders { { "Content-Type", "x/foo" } });
            try
            {
                client.Get("/respond-with-bar");
                Assert.Fail("Expected UnexpectedResponseHeaderException");
            }
            catch (UnexpectedResponseHeaderException e)
            {
                Assert.That(e.Key, Is.EqualTo("Content-Type"));
                Assert.That(e.ExpectedValue, Is.EqualTo("x/foo"));
                Assert.That(e.ActualValue, Is.EqualTo("x/bar"));
                Assert.That(e.Message, Is.EqualTo("Expected response header 'Content-Type' to have the value 'x/foo', but it had the value 'x/bar'"));
            }
        }

        [Test]
        public void DoesNotThrowWhenExpectedResponseHeadersAreSetToExpectedValues()
        {
            _server.OnGet("/respond-with-foo").Respond((req, res) => res.Headers["x"] = "foo");
            var client = new RestClient(BaseAddress, new ExpectResponseHeaders { { "x", "foo" } });
            Assert.That(() => client.Get("/respond-with-foo"), Throws.Nothing);
        }

        [Test]
        public void IsInstantiableWithNoArguments()
        {
            Assert.That(new RestClient().Url, Is.EqualTo(null));
        }

        [Test]
        public void IsInstantiableWithStringAsUrl()
        {
            Assert.That(new RestClient("http://www.featurist.co.uk/").Url.AbsoluteUri, Is.EqualTo("http://www.featurist.co.uk/"));
        }

        [Test]
        public void AcceptsStarSlashStarByDefault()
        {
            _server.OnGet("/accept").Respond((req, res) => res.Body = req.Headers["Accept"]);
            Assert.That(new RestClient(BaseAddress).Get("/accept").Body, Is.EqualTo("*/*"));
        }

        [Test]
        public void AcceptsGzipAndDeflateEncodingByDefault()
        {
            _server.OnGet("/accept-encoding").Respond((req, res) => { res.Body = req.Headers["Accept-Encoding"]; });
            Assert.That(_client.Get("/accept-encoding").Body, Is.EqualTo("gzip, deflate"));
        }

        [Test]
        public void AddsContentHeadersToContent()
        {
            _server.OnPut("/testput").AddHandler((context, next) =>
                {
                    Assert.That(context.Request.Headers["content-encoding"], Is.EqualTo("gzip"));
                    next();
                });

            var content = new StreamBodyContent(new MemoryStream(Encoding.UTF8.GetBytes("Test")), "text/plain");
            content.Headers.Add("Content-Encoding", "gzip");
            _client.Put("/testput", content);
        }

        private class BogusOption : PipelineOption
        {
        }
    }
}
