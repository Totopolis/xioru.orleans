using Xioru.Grain.AbstractGrain;

namespace Xioru.Grain.User;

public class UserState : AbstractGrainState
{
    public string Login { get; set; } = default!;
}
