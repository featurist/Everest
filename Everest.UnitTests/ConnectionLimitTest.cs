using NUnit.Framework;
using SelfishHttp;

namespace Everest.UnitTests
{
    [TestFixture]
    public class ConnectionLimitTest
    {
        private const string BaseAddress = "http://localhost:18754";
        private Server _server;

        [SetUp]
        public void StartServer()
        {
            _server = new Server(18754);
            _server.OnGet("/accept").Respond((req, res) => res.Body = "Body");
        }

        [TearDown]
        public void StopServer()
        {
            _server.Stop();
        }

        [Test]
        public void ManyConnectionsDoesNotThrowHttpRequestException()
        {
            var client = new RestClient(BaseAddress);
            Assert.DoesNotThrow(() =>
            {
                // The max number of allowed outbound http requests on my windows appears to be 16336...
                for (var i = 0; i < 17000; i++)
                {
                    using (var response = client.Get("/accept"))
                    {
                        Assert.AreEqual(response.Body, "Body");
                    }
                }
            });
        }

    }
}
