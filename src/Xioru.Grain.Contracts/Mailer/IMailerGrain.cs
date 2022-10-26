using Orleans;

namespace Xioru.Grain.Contracts.Mailer;

public interface IMailerGrain : IGrainWithGuidKey
{
    Task Send(string email, string body, string subject = "");

    Task SendAdministrative(string body, string subject = "");
}
