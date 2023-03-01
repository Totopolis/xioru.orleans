namespace Xioru.Grain.ProjectRegistry;

[GenerateSerializer]
public class ClusterRegistryState
{
    [Id(0)]
    public List<ProjectDetails> RegistryDetails { get; set; } = new List<ProjectDetails>();
}

[GenerateSerializer]
public record class ProjectDetails(string Name, Guid Id);
