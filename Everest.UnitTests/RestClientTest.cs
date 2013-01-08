using System.Collections.Generic;
using System.Net;
using Everest.Content;
using Everest.Headers;
using Everest.Pipeline;
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
            _client = new RestClient();
        }

        [TearDown]
        public void TearDown()
        {
            _server.Stop();
        }

        [Test]
        public void ReturnsNewResourceAfterEachRequest()
        {
            var fooResource = _client.Get(BaseAddress + "/foo");
            var fooBody = fooResource.Body;
            Assert.That(fooBody, Is.EqualTo("foo!"));

            var barResource = fooResource.Get("foo/bar");
            var barBody = barResource.Body;
            Assert.That(barBody, Is.EqualTo("foo bar?"));
        }

        [Test]
        public void FollowsLinksRelativeToResourceEvenAfterRedirect()
        {
            _server.OnGet("/redirect").RedirectTo("/foo");
            var body = _client.Get(BaseAddress + "/redirect").Get("foo/bar").Body;
            Assert.That(body, Is.EqualTo("foo bar?"));
        }

        [Test]
        public void MakesPutRequests()
        {
            _server.OnPut("/foo").RespondWith(requestBody => "putted " + requestBody);
            var body = _client.Put(BaseAddress + "/foo", "body").Body;
            Assert.That(body, Is.EqualTo("putted body"));
        }

        [Test]
        public void ExposesResponseHeaders()
        {
            _server.OnGet("/whaa").Respond((req, res) => { res.Headers["X-Custom"] = "my custom header"; });

            var response = _client.Get(BaseAddress + "/whaa", ExpectStatus.OK);
            Assert.That(response.Headers["X-Custom"], Is.EqualTo("my custom header"));
        }

        [Test]
        public void ExposesContentHeadersInTheSameCollectionAsOtherResponseHeaders()
        {
            _server.OnGet("/contentType").Respond((req, res) => { res.Headers["Content-Type"] = "x/foo"; });

            var response = _client.Get(BaseAddress + "/contentType");
            Assert.That(response.Headers.ContainsKey("Content-Type"));
            Assert.That(response.Headers["Content-Type"], Is.EqualTo("x/foo"));
        }

        [Test]
        public void MakesOptionsRequests()
        {
            _server.OnOptions("/whaa").RespondWith("options!");
            var body = _client.Options(BaseAddress + "/whaa", ExpectStatus.OK).Body;
            Assert.That(body, Is.EqualTo("options!"));
        }

        [Test]
        public void MakesPostRequests()
        {
            _server.OnPost("/foo").RespondWith(requestBody => "posted " + requestBody);
            var body = _client.Post(BaseAddress + "/foo", "body", ExpectStatus.OK).Body;
            Assert.That(body, Is.EqualTo("posted body"));
        }

        [Test]
        public void MakesPostRequestsWithBodyContent()
        {
            _server.OnPost("/foo").RespondWith(requestBody => "posted " + requestBody);
            var body = _client.Post(BaseAddress + "/foo", new StringBodyContent("body"), ExpectStatus.OK).Body;
            Assert.That(body, Is.EqualTo("posted body"));
        }

        [Test]
        public void MakesHeadRequests()
        {
            _server.OnHead("/foo").Respond((req, res) => res.StatusCode = 303);
            var response = _client.Head(BaseAddress + "/foo", new ExpectStatus(HttpStatusCode.SeeOther));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.SeeOther));
        }

        [Test]
        public void MakesPostRequestsToSelf()
        {
            _server.OnGet("/self").RespondWith("good on ya");
            _server.OnPost("/self").RespondWith(requestBody => "posted " + requestBody);
            var response = _client.Get(BaseAddress + "/self");
            var body = response.Post("body").Body;
            Assert.That(body, Is.EqualTo("posted body"));
        }

        [Test]
        public void GetExpects200RangeStatusByDefault()
        {
            Assert.That(() => _client.Get(BaseAddress + "/non-existent"), Throws.Exception);
        }

        [Test]
        public void GetExpectStatusIsOverridable()
        {
            Assert.That(() => _client.Get(BaseAddress + "/foo", new ExpectStatus(HttpStatusCode.InternalServerError)), Throws.InstanceOf<UnexpectedStatusException>());
        }

        [Test]
        public void PutExpectStatusIsOverridable()
        {
            Assert.That(() => _client.Put(BaseAddress + "/foo", "oops", new ExpectStatus(HttpStatusCode.InternalServerError)), Throws.InstanceOf<UnexpectedStatusException>());
        }

        [Test]
        public void GetTakesHeaders()
        {
            _server.OnGet("/headers").Respond((req, res) => res.Body = req.Headers["x-something"]);
            var response = _client.Get(BaseAddress + "/headers", new Dictionary<string, string> { { "x-something", "wowzer" } });
            Assert.That(response.Body, Is.EqualTo("wowzer"));
        }

        [Test]
        public void ThrowsWhenUnsupportedPerRequestOptionsAreSupplied()
        {
            Assert.That(() => _client.Get(BaseAddress + "/foo", new BogusOption()), Throws.InstanceOf<UnsupportedOptionException>());
        }

        [Test]
        public void ThrowsWhenUnsupportedAmbientOptionsAreSupplied()
        {
            _server.OnGet("/blah").RespondWith("ok!");
            Assert.That(() => new RestClient(new BogusOption()).Get(BaseAddress + "/blah"), Throws.InstanceOf<UnsupportedOptionException>());
        }

        [Test]
        public void AppliesPipelineOptionsToSubsequentRequests()
        {
            _server.OnGet("/headers").Respond((req, res) => res.Body = req.Headers["x-per-client"]);

            var client = new RestClient(new SetRequestHeaders(new Dictionary<string, string> { { "x-per-client", "x" } }));
            var firstResponse = client.Get(BaseAddress + "/headers");
            Assert.That(firstResponse.Body, Is.EqualTo("x"));
            var secondResponse = firstResponse.Get("/headers");
            Assert.That(secondResponse.Body, Is.EqualTo("x"));
        }

        [Test]
        public void AppliesPipelineOptionsPerRequest()
        {
            _server.OnGet("/headers").Respond((req, res) => res.Body = req.Headers["x-per-client"]);

            var client = new RestClient();
            var firstResponse = client.Get(BaseAddress + "/headers", new SetRequestHeaders(new Dictionary<string, string> { { "x-per-client", "x" } }));
            Assert.That(firstResponse.Body, Is.EqualTo("x"));
            var secondResponse = firstResponse.Get("/headers");
            Assert.That(secondResponse.Body, Is.EqualTo(""));
        }

        [Test]
        public void ProvidesAConvenientWayToSetAcceptHeader()
        {
            _server.OnGet("/accept").Respond((req, res) => res.Body = req.Headers["Accept"]);
            var client = new RestClient();
            var response = client.Get(BaseAddress + "/accept", new Accept("foo/bar"));
            Assert.That(response.Body, Is.EqualTo("foo/bar"));
        }


        [Test]
        public void ThrowsWhenExpectedResponseHeaderIsNotSet()
        {
            _server.OnGet("/respond-with-bar").Respond((req, res) => res.Headers["unknown"] = "bar");
            var client = new RestClient(new ExpectResponseHeaders { { "x", "foo" }});
            try
            {
                client.Get(BaseAddress + "/respond-with-bar");
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
            var client = new RestClient(new ExpectResponseHeaders { { "x", "foo" }});
            try
            {
                client.Get(BaseAddress + "/respond-with-bar");
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
        public void DoesNotThrowWhenExpectedResponseHeadersAreSetToExpectedValues()
        {
            _server.OnGet("/respond-with-foo").Respond((req, res) => res.Headers["x"] = "foo");
            var client = new RestClient(new ExpectResponseHeaders { { "x", "foo" } });
            Assert.That(() => client.Get(BaseAddress + "/respond-with-foo"), Throws.Nothing);
        }

        private class BogusOption : PipelineOption
        {
        }
    }
}
