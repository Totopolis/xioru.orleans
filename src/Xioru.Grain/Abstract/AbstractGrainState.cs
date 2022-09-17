namespace Xioru.Grain.AbstractGrain;

public abstract class AbstractGrainState
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public Guid ProjectId { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public string Description { get; set; } = default!;

    public List<string> Tags { get; set; } = new();
}
