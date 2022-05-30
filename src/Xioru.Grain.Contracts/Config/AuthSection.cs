namespace Xioru.Grain.Contracts.Config
{
    public class AuthSection
    {
        public const string SectionName = "Auth";

        public string Issuer { get; set; } = default!;

        public string Audience { get; set; } = default!;

        public string RefreshAudience { get; set; } = default!;

        public TimeSpan Lifetime { get; set; }

        public TimeSpan RefreshLifetime { get; set; }

        public string SecretKey { get; set; } = default!;
    }
}