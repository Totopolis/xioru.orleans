namespace Xioru.Grain.ClusterRegistry;

[GenerateSerializer]
public class ClusterRegistryState
{
    [Id(0)]
    public List<ProjectDetails> RegistryDetails { get; set; } = new List<ProjectDetails>();
}
