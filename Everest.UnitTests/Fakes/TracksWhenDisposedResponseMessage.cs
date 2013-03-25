using System.Net.Http;

namespace Everest.UnitTests.Fakes
{
    public class TracksWhenDisposedResponseMessage : HttpResponseMessage
    {
        public TracksWhenDisposedResponseMessage()
        {
            DisposeCount = 0;
            RequestMessage = new HttpRequestMessage();
        }

        protected override void Dispose(bool disposing)
        {
            DisposeCount++;
            base.Dispose(disposing);
        }

        public int DisposeCount { get; private set; }
    }
}