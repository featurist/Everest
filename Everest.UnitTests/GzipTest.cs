using Everest.Headers;
using NUnit.Framework;

namespace Everest.UnitTests
{
    [TestFixture]
    public class GzipTest
    {
        [Test]
        public void ReadsGzippedContent()
        {
            var response = new RestClient().Get("http://httpbin.org/gzip");
            Assert.That(response.Body, Is.StringContaining("gzipped"));
        }
    }
}
