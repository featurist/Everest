using System;
using System.Security.Policy;
using System.Threading;
using Everest.Caching;
using Everest.Headers;
using Everest.SystemNetHttp;
using NUnit.Framework;

namespace Everest.UnitTests
{
    [TestFixture]
    public class CachingTests
    {
        private CachingServer _server;
        private RestClient _client;

        [SetUp]
        public void SetUp()
        {
            _server = new CachingServer(12345);
            _client = new RestClient(_server.Url, new CachePolicy { Cache = true });
        }

        [TearDown]
        public void TearDown()
        {
            _server.Stop();
        }

        [Test]
        public void ShouldGetCachedResponsesWhenCacheHasNotExpired()
        {
            _server.CacheControl = "public, must-revalidate, max-age=10";
            ShouldGetFreshResource("request 1");
            ShouldGetCachedResource("request 1");
        }

        [Test]
        public void ShouldGetFreshResponsesWhenCacheHasExpired()
        {
            _server.CacheControl = "public, must-revalidate, max-age=1";
            ShouldGetFreshResource("request 1");
            Wait(1);
            ShouldGetFreshResource("request 2");
        }

        private void Wait(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

        private void ShouldGetCachedResource(string body)
        {
            var numberOfRequests = _server.NumberOfRequests;
            var response = _client.Get("");
            Assert.That(_server.NumberOfRequests, Is.EqualTo(numberOfRequests));
            Assert.That(response.Body, Is.EqualTo(body));
        }

        private void ShouldGetFreshResource(string body)
        {
            var numberOfRequests = _server.NumberOfRequests;
            var response = _client.Get("");
            Assert.That(response.Body, Is.EqualTo(body));
            Assert.That(_server.NumberOfRequests, Is.EqualTo(numberOfRequests + 1));
        }
    }
}