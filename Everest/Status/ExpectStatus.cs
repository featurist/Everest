using System.Linq;
using System.Net;

namespace Everest.Status
{
    public class ExpectStatus : StatusAcceptability
    {
        private readonly HttpStatusCode[] _acceptableStatuses;

        public ExpectStatus(params HttpStatusCode?[] acceptableStatuses)
        {
            _acceptableStatuses = acceptableStatuses.Where(s => s.HasValue).Select(s => s.Value).ToArray();
        }

        public bool IsStatusAcceptable(HttpStatusCode status)
        {
            return _acceptableStatuses.Length == 0 || _acceptableStatuses.Any(acceptable => acceptable == status);
        }

        public string DescribeAcceptableStatuses()
        {
            return _acceptableStatuses.Length > 1 ? DescribeMultiple() : DescribeSingle();
        }

        private string DescribeSingle()
        {
            return _acceptableStatuses[0].ToString();
        }

        private string DescribeMultiple()
        {
            return string.Format("one of ({0})", string.Join(", ", _acceptableStatuses.ToArray()));
        }

        public static ExpectStatus Created
        {
            get { return new ExpectStatus(HttpStatusCode.Created); }
        }

        public static ExpectStatus OK
        {
            get { return new ExpectStatus(HttpStatusCode.OK); }
        }

        public static ExpectStatus NotFound
        {
            get { return new ExpectStatus(HttpStatusCode.NotFound); }
        }

        public static ExpectStatus IgnoreStatus
        {
            get { return new ExpectStatus(); }
        }
    }
}