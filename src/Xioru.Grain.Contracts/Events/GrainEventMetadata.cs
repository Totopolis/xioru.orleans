namespace Xioru.Grain.Contracts.Messages;

[GenerateSerializer]
public class GrainEventMetadata
{
    [Id(0)]
    public Guid ProjectId { get; set; } = Guid.Empty;

    [Id(1)]
    public string GrainType { get; set; } = default!;

    [Id(2)]
    public Guid GrainId { get; set; } = Guid.Empty;

    [Id(3)]
    public string GrainName { get; set; } = default!;

    [Id(4)]
    public DateTime CreatedUtc { get; set; }
}
