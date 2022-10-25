namespace Xioru.Grain.Contracts.ApiKeyReadModel;

public class ApiKeyDescription
{
    public DateTime Created { get; set; }

    public Guid ApiKey { get; set; }

    public Guid ProjectId { get; set; }
}
