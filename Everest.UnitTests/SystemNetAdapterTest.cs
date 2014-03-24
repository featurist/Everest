using System;
using Everest.Caching;
using Everest.Compression;
using Everest.Pipeline;
using Everest.Redirection;
using Everest.SystemNetHttp;
using Everest.Timing;
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
        public void PassesTheTimeoutOptionToTheUnderlyingClient()
        {
            var timeSpan = TimeSpan.FromDays(123);
            var adapter = CreateAdapter(new RequestTimeout(timeSpan));
            Assert.That(adapter.Client.Timeout, Is.EqualTo(timeSpan));
        }

        [Test]
        public void WhenPipelineOptionsAreIdentialThenTheSameClientIsReused()
        {
            GetsTheSameClientWithPipelineOption(AutoRedirect.DoNotAutoRedirect);
            GetsTheSameClientWithPipelineOption(AutoRedirect.AutoRedirectAndForwardAuthorizationHeader);
            GetsTheSameClientWithPipelineOption(AutoRedirect.AutoRedirectButDoNotForwardAuthorizationHeader);
            GetsTheSameClientWithPipelineOption(new CachePolicy { Cache = false });
            GetsTheSameClientWithPipelineOption(new CachePolicy { Cache = true });
            GetsTheSameClientWithPipelineOption(new CachePolicy { Cache = true }, AutoRedirect.DoNotAutoRedirect);
            GetsTheSameClientWithPipelineOption(new CachePolicy { Cache = true }, AutoRedirect.AutoRedirectButDoNotForwardAuthorizationHeader);
            GetsTheSameClientWithPipelineOption(new AcceptEncoding { AcceptGzipAndDeflate = true });
        }

        [Test]
        public void ReturnsDifferentClientWhenPipelineOptionsAreDifferent()
        {
            GetsDifferentClientsWithDifferentOptions(new PipelineOption[] {AutoRedirect.DoNotAutoRedirect}, new PipelineOption[] {AutoRedirect.AutoRedirectAndForwardAuthorizationHeader});
            GetsDifferentClientsWithDifferentOptions(new PipelineOption[] { new CachePolicy { Cache = false } }, new PipelineOption[] { new CachePolicy { Cache = true } });
            GetsDifferentClientsWithDifferentOptions(new PipelineOption[] { new CachePolicy { Cache = false } }, new PipelineOption[] {AutoRedirect.AutoRedirectAndForwardAuthorizationHeader, new CachePolicy { Cache = false } });
        }

        private void GetsDifferentClientsWithDifferentOptions(PipelineOption[] firstOptions, PipelineOption[] secondOptions)
        {
            Assert.That(CreateAdapter(firstOptions), Is.Not.SameAs(CreateAdapter(secondOptions)));
        }

        void GetsTheSameClientWithPipelineOption(params PipelineOption[] options)
        {
            Assert.That(CreateAdapter(options), Is.SameAs(CreateAdapter(options)));
        }

        private SystemNetHttpClientAdapter CreateAdapter(params PipelineOption[] pipelineOptions)
        {
            return (SystemNetHttpClientAdapter)_adapterFactory.CreateClient(new PipelineOptions(pipelineOptions));
        }
    }
}
