namespace Xioru.Messaging.Contracts.Command
{
    [Serializable]
    public class CommandLogicErrorException : Exception
    {
        public CommandLogicErrorException() { }
        public CommandLogicErrorException(string message) : base(message) { }
        public CommandLogicErrorException(string message, Exception inner) : base(message, inner) { }
        protected CommandLogicErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
