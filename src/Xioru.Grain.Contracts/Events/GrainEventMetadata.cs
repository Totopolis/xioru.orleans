namespace Xioru.Grain.Contracts.Messages;

public class GrainEventMetadata
{
    public Guid ProjectId { get; set; } = Guid.Empty;

    public string GrainType { get; set; } = default!;

    public Guid GrainId { get; set; } = Guid.Empty;

    public string GrainName { get; set; } = default!;

    public DateTime CreatedUtc { get; set; }
}
