namespace Xioru.Messaging.Contracts.Command;

[Serializable]
public class CommandSyntaxErrorException : Exception
{
    public CommandSyntaxErrorException() { }
    public CommandSyntaxErrorException(string message) : base(message) { }
    public CommandSyntaxErrorException(string message, Exception inner) : base(message, inner) { }
    protected CommandSyntaxErrorException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
