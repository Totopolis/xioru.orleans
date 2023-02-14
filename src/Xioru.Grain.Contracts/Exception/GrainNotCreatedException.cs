namespace Xioru.Grain.Contracts.Exception;

[Serializable]
[GenerateSerializer]
public class GrainNotCreatedException : System.Exception
{
	public GrainNotCreatedException() { }
	public GrainNotCreatedException(string message) : base(message) { }
	public GrainNotCreatedException(string message, System.Exception inner) : base(message, inner) { }
	protected GrainNotCreatedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
