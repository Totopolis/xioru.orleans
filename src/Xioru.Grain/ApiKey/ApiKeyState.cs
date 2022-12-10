using Xioru.Grain.AbstractGrain;

namespace Xioru.Grain.ApiKey;

public class ApiKeyState : AbstractGrainState
{
    public Guid ApiKey { get; set; }
}
