namespace Xioru.Grain.Project;

[GenerateSerializer]
public class ProjectState
{
    public string Name { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTime CreatedUtc { get; set; } = DateTime.MinValue;
}
