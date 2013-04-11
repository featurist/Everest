using System;
using SelfishHttp;

namespace Everest.UnitTests
{
    public class CachingServer
    {
        private Server _server;
        private int _version;
        public string CacheControl { get; set; }
        public string Url { get; private set; }
        public int NumberOfRequests { get; private set; }

        public CachingServer(int port)
        {
            _server = new Server(port);
            string path = "/" + Guid.NewGuid().ToString();
            Url = "http://localhost:" + port + path;
            _version = 0;
            NumberOfRequests = 0;

            _server.OnGet(path).Respond((req, res) =>
            {
                _version++;
                NumberOfRequests++;
                res.Headers["Cache-Control"] = CacheControl;
                res.Body = "request " + _version;
            });
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}