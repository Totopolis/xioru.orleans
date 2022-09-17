namespace Xioru.Grain.Contracts.Exception;

[Serializable]
public class AccountBadPasswordException : System.Exception
{
	public AccountBadPasswordException() { }
	public AccountBadPasswordException(string message) : base(message) { }
	public AccountBadPasswordException(string message, System.Exception inner) : base(message, inner) { }
	protected AccountBadPasswordException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
