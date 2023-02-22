using Xioru.Grain.Contracts.ProjectRegistry;

namespace Xioru.Grain.ProjectRegistry;

[GenerateSerializer]
public class ProjectRegistryState
{
    [Id(0)]
    public string ProjectName { get; set; } = default!;

    [Id(1)]
    public List<GrainRegistryDetails> RegistryDetails { get; set; } = new List<GrainRegistryDetails>();
}
