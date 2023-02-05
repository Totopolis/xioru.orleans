namespace Xioru.Grain.Contracts.ApiKeyReadModel;

[GenerateSerializer]
public class ApiKeyDescription
{
    [Id(0)]
    public DateTime Created { get; set; }

    [Id(1)]
    public Guid ApiKey { get; set; }

    [Id(2)]
    public Guid ProjectId { get; set; }
}
