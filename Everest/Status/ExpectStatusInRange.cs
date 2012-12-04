using System.Net;

namespace Everest.Status
{
    public class ExpectStatusInRange : StatusAcceptability
    {
        private readonly int _minStatus;
        private readonly int _maxStatus;

        public ExpectStatusInRange(int minStatus, int maxStatus)
        {
            _minStatus = minStatus;
            _maxStatus = maxStatus;
        }

        public bool IsStatusAcceptable(HttpStatusCode status)
        {
            var i = (int) status;
            return i >= _minStatus && i <= _maxStatus;
        }

        public string DescribeAcceptableStatuses()
        {
            return string.Format("in the range {0}-{1}", _minStatus, _maxStatus);
        }
    }
}