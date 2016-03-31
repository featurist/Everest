using System;
using Everest.Pipeline;
using SysIWebProxy = System.Net.IWebProxy;

namespace Everest.Proxy
{
    public abstract class IWebProxy
    {
        private readonly SysIWebProxy _proxy;
        protected abstract SysIWebProxy CreateWebProxy(string host);

        public IWebProxy (string host) {
            _proxy = CreateWebProxy(host);
        }

        public SysIWebProxy Proxy {
            get { return _proxy; }
        }
    }
}
