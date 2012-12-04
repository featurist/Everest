using System.Net;
using Everest.Pipeline;

namespace Everest.Status
{
    public interface StatusAcceptability : PipelineOption
    {
        bool IsStatusAcceptable(HttpStatusCode status);
        string DescribeAcceptableStatuses();
    }
}