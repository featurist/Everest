using Everest.Headers;
using NUnit.Framework;

namespace Everest.UnitTests
{
    [TestFixture]
    public class BinaryContentTest
    {
        [Test]
        public void StreamsBinaryContent()
        {
            var response = new RestClient().Get("http://httpbin.org/stream-bytes/1024");

            using ( var stream = response.BodyAsStream)
            {
                Assert.AreEqual(stream.Length,1024);
            }

        }
    }
}
