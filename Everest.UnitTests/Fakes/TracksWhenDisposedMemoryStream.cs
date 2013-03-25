using System.IO;

namespace Everest.UnitTests.Fakes
{
    public class TracksWhenDisposedMemoryStream : MemoryStream
    {
        public int DisposedCount { get; private set; }

        protected override void Dispose(bool disposing)
        {
            DisposedCount++;
            base.Dispose(disposing);
        }
    }
}