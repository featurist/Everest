using Everest.Pipeline;

namespace Everest.Redirection
{
    public class AutoRedirect : PipelineOption
    {
        public static readonly PipelineOption DoNotAutoRedirect = new AutoRedirect { EnableAutomaticRedirection = false, ForwardAuthorizationHeader = false };
        public static readonly PipelineOption AutoRedirectButDoNotForwardAuthorizationHeader = new AutoRedirect { EnableAutomaticRedirection = true, ForwardAuthorizationHeader = false };
        public static readonly PipelineOption AutoRedirectAndForwardAuthorizationHeader = new AutoRedirect { EnableAutomaticRedirection = true, ForwardAuthorizationHeader = true };

        public bool ForwardAuthorizationHeader { get; set; }
        public bool EnableAutomaticRedirection { get; set; }

        protected bool Equals(AutoRedirect other) {
            return ForwardAuthorizationHeader.Equals(other.ForwardAuthorizationHeader) && EnableAutomaticRedirection.Equals(other.EnableAutomaticRedirection);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AutoRedirect) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (ForwardAuthorizationHeader.GetHashCode()*397) ^ EnableAutomaticRedirection.GetHashCode();
            }
        }
    }
}
