using System;
using System.Threading;
using System.Threading.Tasks;
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
            _server.OnGet("/sleep").Respond(SleepForASecond);
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

        [Test]
        public void ManyConnectionsAreUsedInParallel()
        {
            var client = new RestClient(BaseAddress);
            SleepOnce(client); // warm up
            var startedAt = DateTime.Now;
            Parallel.For(1, 50, i => SleepOnce(client));
            var elapsed = DateTime.Now - startedAt;
            Assert.That(elapsed.TotalSeconds, Is.LessThan(10));
            Console.WriteLine(elapsed.TotalSeconds);
        }

        private static void SleepOnce(Resource client)
        {
            using (var response = client.Get("/sleep")) { Assert.AreEqual(response.Body, "zzz!"); } 
        }

        private static int _counter;

        private static void SleepForASecond(IRequest req, IResponse res)
        {
            Interlocked.Increment(ref _counter);
            var c = _counter;
            Console.WriteLine("starting request " + c);
            Thread.Sleep(1000);
            res.Body = "zzz!";
            Console.WriteLine("finishing request " + c);
        }
    }
}
