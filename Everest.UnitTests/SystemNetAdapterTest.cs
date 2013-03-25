using Everest.Pipeline;
using Everest.Redirection;
using Everest.SystemNetHttp;
using NUnit.Framework;

namespace Everest.UnitTests
{
    [TestFixture]
    public class SystemNetAdapterTest
    {
        [Test]
        public void ReusesTheSameUnderlyingHttpClientUnlessAutoRedirectConfigurationChanges()
        {
            var adapterFactory = new SystemNetHttpClientAdapterFactory();
            var adapter1 = (SystemNetHttpClientAdapter)adapterFactory.CreateClient(new PipelineOptions(new PipelineOption[0]));
            var adapter2 = (SystemNetHttpClientAdapter)adapterFactory.CreateClient(new PipelineOptions(new PipelineOption[0]));
            var adapter3 = (SystemNetHttpClientAdapter)adapterFactory.CreateClient(new PipelineOptions(new [] { AutoRedirect.DoNotAutoRedirect }));
            var adapter4 = (SystemNetHttpClientAdapter)adapterFactory.CreateClient(new PipelineOptions(new [] { AutoRedirect.DoNotAutoRedirect }));
            var adapter5 = (SystemNetHttpClientAdapter)adapterFactory.CreateClient(new PipelineOptions(new [] { AutoRedirect.AutoRedirectAndForwardAuthorizationHeader }));
            var adapter6 = (SystemNetHttpClientAdapter)adapterFactory.CreateClient(new PipelineOptions(new [] { AutoRedirect.AutoRedirectAndForwardAuthorizationHeader }));
            Assert.That(adapter1.Client, Is.SameAs(adapter2.Client));
            Assert.That(adapter3.Client, Is.SameAs(adapter4.Client));
            Assert.That(adapter5.Client, Is.SameAs(adapter6.Client));
            Assert.That(adapter1.Client, Is.Not.SameAs(adapter3.Client));
            Assert.That(adapter1.Client, Is.Not.SameAs(adapter5.Client));
            Assert.That(adapter3.Client, Is.Not.SameAs(adapter5.Client));
        }
    }
}
