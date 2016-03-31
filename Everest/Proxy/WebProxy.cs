using System;
using Everest.Pipeline;
using SysIWebProxy = System.Net.IWebProxy;
using SysWebProxy = System.Net.WebProxy;

namespace Everest.Proxy
{
    public class WebProxy : IWebProxy, PipelineOption
    {
        public WebProxy(string host) : base (host) { }

        protected override SysIWebProxy CreateWebProxy (string host) {
            return new SysWebProxy(host) as SysIWebProxy;
        }
    }
}
