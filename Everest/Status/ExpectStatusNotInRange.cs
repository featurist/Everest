using System.Net;

namespace Everest.Status
{
    public class ExpectStatusNotInRange : StatusAcceptability
    {
        private readonly int _minStatus;
        private readonly int _maxStatus;

        public ExpectStatusNotInRange(int minStatus, int maxStatus)
        {
            _minStatus = minStatus;
            _maxStatus = maxStatus;
        }

        public bool IsStatusAcceptable(HttpStatusCode status)
        {
            var i = (int)status;
            return i < _minStatus || i > _maxStatus;
        }

        public string DescribeAcceptableStatuses()
        {
            return string.Format("not in the range {0}-{1}", _minStatus, _maxStatus);
        }
    }
}