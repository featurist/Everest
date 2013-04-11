using Everest.Headers;

namespace Everest.Compression
{
    public class AcceptEncoding : RequestHeader
    {
        public AcceptEncoding(string value) : base("Accept-Encoding", value)
        {
        }

        public static RequestHeader None
        {
            get { return new RequestHeader("Accept-Encoding", null); }
        }

        public static RequestHeader Gzip
        {
            get { return new RequestHeader("Accept-Encoding", "gzip"); }
        }

        public static RequestHeader GzipAndDeflate
        {
            get { return new RequestHeader("Accept-Encoding", "gzip, deflate"); }
        }
    }
}
