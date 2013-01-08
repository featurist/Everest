using System;

namespace Everest.Headers
{
    public class UnexpectedResponseHeaderException : Exception
    {
        public UnexpectedResponseHeaderException(string key, string expectedValue, string actualValue = null) 
        {
            Key = key;
            ExpectedValue = expectedValue;
            ActualValue = actualValue;
        }

        public string Key { get; private set; }
        public string ExpectedValue { get; private set; }
        public string ActualValue { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("Expected response header '{0}' to have the value '{1}', but it had the value '{2}'", Key, ExpectedValue, ActualValue);
            }
        }

    }
}
