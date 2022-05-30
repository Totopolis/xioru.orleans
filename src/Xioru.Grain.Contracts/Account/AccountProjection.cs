namespace Xioru.Grain.Contracts.Account
{
    public record AccountProjection(
        string AccountId,
        Guid[] AccessibleProjects);
}
