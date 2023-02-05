namespace Xioru.Grain.AbstractGrain;

[GenerateSerializer]
public abstract class AbstractGrainState
{
    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public string Name { get; set; } = default!;

    [Id(2)]
    public Guid ProjectId { get; set; } = default!;

    [Id(3)]
    public string DisplayName { get; set; } = default!;

    [Id(4)]
    public string Description { get; set; } = default!;

    [Id(5)]
    public List<string> Tags { get; set; } = new();

    [Id(6)]
    public DateTime CreatedUtc { get; set; } = DateTime.MinValue;

    [Id(7)]
    public DateTime UpdatedUtc { get; set; } = DateTime.MinValue;
}
