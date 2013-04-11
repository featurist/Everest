using Everest.Pipeline;
using Everest.Redirection;
using Everest.SystemNetHttp;
using NUnit.Framework;

namespace Everest.UnitTests
{
    [TestFixture]
    public class SystemNetAdapterTest
    {
        private SystemNetHttpClientAdapterFactory _adapterFactory;
        
        [SetUp]
        public void CreateFactory()
        {
            _adapterFactory = new SystemNetHttpClientAdapterFactory();
        }

        [Test]
        public void WhenPipelineOptionsAreIdentialThenTheSameClientIsReused()
        {
            var blank = CreateClient();
            var blank2 = CreateClient();
            var noRedirect = CreateClient(AutoRedirect.DoNotAutoRedirect);
            var noRedirect2 = CreateClient(AutoRedirect.DoNotAutoRedirect);
            var autoRedirect = CreateClient(AutoRedirect.AutoRedirectAndForwardAuthorizationHeader);
            var autoRedirect2 = CreateClient(AutoRedirect.AutoRedirectAndForwardAuthorizationHeader);
            Assert.That(blank.Client, Is.SameAs(blank2.Client));
            Assert.That(noRedirect.Client, Is.SameAs(noRedirect2.Client));
            Assert.That(autoRedirect.Client, Is.SameAs(autoRedirect2.Client));
            Assert.That(blank.Client, Is.Not.SameAs(noRedirect.Client));
            Assert.That(blank.Client, Is.Not.SameAs(autoRedirect.Client));
            Assert.That(noRedirect.Client, Is.Not.SameAs(autoRedirect.Client));
        }

        private SystemNetHttpClientAdapter CreateClient(params PipelineOption[] pipelineOptions)
        {
            return (SystemNetHttpClientAdapter)_adapterFactory.CreateClient(new PipelineOptions(pipelineOptions));
        }
    }
}
