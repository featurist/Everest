using System;
using Everest.Timing;
using NUnit.Framework;

namespace Everest.UnitTests
{
    [TestFixture]
    public class TimeoutTest
    {
        private RestClient _client;

        [SetUp]
        public void CreateClient()
        {
            _client = new RestClient("http://httpbin.org");
        }

        [Test]
        public void WhenTheTimeoutIsLongerThanTheResponseTimeThenItDoesNotThrow()
        {
            _client.Get("/delay/1", new RequestTimeout(TimeSpan.FromSeconds(2)));
        }

        [Test]
        public void WhenTheTimeoutIsShorterThanTheResponseTimeThenItThrows()
        {
            Assert.That(() => { _client.Get("/delay/2", new RequestTimeout(TimeSpan.FromSeconds(1))); }, Throws.InstanceOf<TimeoutException>());
        }
    }
}
