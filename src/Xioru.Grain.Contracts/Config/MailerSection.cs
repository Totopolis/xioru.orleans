namespace Xioru.Grain.Contracts.Config
{
    public class MailerSection
    {
        public const string SectionName = "Mailer";

        public string Host { get; set; } = default!;

        public int Port { get; set; }

        public bool UseSsl { get; set; }

        public string UserName { get; set; } = default!;

        public string Password { get; set; } = default!;

        // like no-reply@domain.com
        public string SenderMail { get; set; } = default!;
    }
}
