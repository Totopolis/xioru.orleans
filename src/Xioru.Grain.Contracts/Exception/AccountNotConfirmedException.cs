namespace Xioru.Grain.Contracts.Exception;

[Serializable]
public class AccountNotConfirmedException : System.Exception
{
	public AccountNotConfirmedException() { }
	public AccountNotConfirmedException(string message) : base(message) { }
	public AccountNotConfirmedException(string message, System.Exception inner) : base(message, inner) { }
	protected AccountNotConfirmedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
