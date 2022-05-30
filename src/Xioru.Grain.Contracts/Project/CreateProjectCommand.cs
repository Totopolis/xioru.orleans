namespace Xioru.Grain.Contracts.Project
{
    public class CreateProjectCommand
    {
        public string Name { get; init; } = default!;

        public string DisplayName { get; init; } = default!;

        public string Description { get; init; } = default!;

        // TODO: roles
    }
}
