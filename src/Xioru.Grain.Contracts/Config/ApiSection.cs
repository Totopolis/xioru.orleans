namespace Xioru.Grain.Contracts.Config
{
    public class ApiSection
    {
        public const string SectionName = "Api";

        public string HostName { get; set; } = default!;
    }
}