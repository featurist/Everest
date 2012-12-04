using Everest.Pipeline;

namespace Everest.Auth
{
    public class BasicAuth : PipelineOption
    {
        public readonly string Username;
        public readonly string Password;

        public BasicAuth(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
