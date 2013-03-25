using System;
using Everest.Content;
using Everest.Pipeline;
using Everest.Status;
using Everest.UnitTests.Fakes;
using NUnit.Framework;

namespace Everest.UnitTests
{
    [TestFixture]
    public class DisposeTest
    {
        [Test]
        public void DisposingAResponseDisposesTheUnderlyingResponse()
        {
            var client = new TracksWhenDisposedClient();
            var clientFactory = new StubAdapterFactory(client);
            var restClient = new RestClient(new Uri("http://localhost"), clientFactory, new PipelineOption[0]);
            var response = restClient.Get("oops");
            Assert.That(client.Responses.Count, Is.EqualTo(1));
            Assert.That(client.Responses[0].DisposeCount, Is.EqualTo(0));
            response.Dispose();
            Assert.That(client.Responses[0].DisposeCount, Is.EqualTo(1));
        }

        [Test]
        public void RequestContentStreamIsDisposedAutomatically()
        {
            var client = new RestClient("http://localhost/abc");
            var memoryStream = new TracksWhenDisposedMemoryStream();
            client.Post("something", new StreamBodyContent(memoryStream, "foo/bar"), ExpectStatus.IgnoreStatus);
            Assert.That(memoryStream.DisposedCount, Is.EqualTo(1));
        }
    }
}
