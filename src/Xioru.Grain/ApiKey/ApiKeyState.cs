using Xioru.Grain.AbstractGrain;

namespace Xioru.Grain.ApiKey
{
    public class ApiKeyState : AbstractGrainState
    {
        public DateTime Created { get; set; }

        public Guid ApiKey { get; set; }
    }
}
