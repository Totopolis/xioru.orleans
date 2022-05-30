namespace Xioru.Messaging.Contracts.Command
{
    [Serializable]
    public class CommandInternalErrorException : Exception
    {
        public CommandInternalErrorException() { }
        public CommandInternalErrorException(string message) : base(message) { }
        public CommandInternalErrorException(string message, Exception inner) : base(message, inner) { }
        protected CommandInternalErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
