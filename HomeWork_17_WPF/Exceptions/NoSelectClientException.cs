using System;

namespace HomeWork_17_WPF.Exceptions
{
    public class NoSelectClientException : Exception
    {
        private string messageDetails = string.Empty;
        public NoSelectClientException()
        {
        }

        public NoSelectClientException(string message) : base(message)
        {
            messageDetails = message;
        }
        public override string Message => messageDetails;
    }
}
