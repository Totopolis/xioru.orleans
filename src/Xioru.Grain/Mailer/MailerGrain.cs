using DnsClient.Internal;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using System.ComponentModel.DataAnnotations;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Config;
using Xioru.Grain.Contracts.Mailer;

namespace Xioru.Grain.Mailer;

[ImplicitStreamSubscription(GrainConstants.MailerStreamNamespace)]
public class MailerGrain :
    Orleans.Grain,
    IMailerGrain,
    IStreamSubscriptionObserver,
    IAsyncObserver<EmailEvent>
{
    private readonly ILogger<MailerGrain> _logger;
    private readonly MailerSection _mailerConfig;

    public MailerGrain(
        ILogger<MailerGrain> logger,
        IOptions<MailerSection> mailerOptions)
    {
        _logger = logger;
        _mailerConfig = mailerOptions.Value;
    }

    public async Task Send(string email, string body, string subject = "")
    {
        if (_mailerConfig == null ||
            !new EmailAddressAttribute().IsValid(email) ||
            string.IsNullOrWhiteSpace(body))
        {
            return;
        }

        var streamProvider = GetStreamProvider(GrainConstants.StreamProviderName);
        var stream = streamProvider.GetStream<EmailEvent>(
                streamId: GrainConstants.MailerStreamId,
                streamNamespace: GrainConstants.MailerStreamNamespace);

        var emailEvent = new EmailEvent
        {
            Email = email,
            Body = body,
            Subject = subject
        };

        await stream.OnNextAsync(emailEvent);
    }

    public async Task SendAdministrative(string body, string subject = "")
    {
        if (_mailerConfig == null ||
            !_mailerConfig.AdminEmails.Any() ||
            string.IsNullOrWhiteSpace(body))
        {
            return;
        }

        var streamProvider = GetStreamProvider(GrainConstants.StreamProviderName);
        var stream = streamProvider.GetStream<EmailEvent>(
                streamId: GrainConstants.MailerStreamId,
                streamNamespace: GrainConstants.MailerStreamNamespace);

        foreach (var it in _mailerConfig.AdminEmails)
            if (!new EmailAddressAttribute().IsValid(it))
            {
                var emailEvent = new EmailEvent
                {
                    Email = it,
                    Body = body,
                    Subject = subject
                };

                await stream.OnNextAsync(emailEvent);
            }
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogError(ex, "Error at consume stream in MailerGrain");
        return Task.CompletedTask;
    }

    public async Task OnNextAsync(EmailEvent item, StreamSequenceToken token)
    {
        if (_mailerConfig == null ||
            string.IsNullOrWhiteSpace(_mailerConfig.SenderMail) ||
            string.IsNullOrWhiteSpace(_mailerConfig.Host) ||
            string.IsNullOrWhiteSpace(_mailerConfig.UserName) ||
            string.IsNullOrWhiteSpace(_mailerConfig.Password))
        {
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            name: _mailerConfig.SenderMail,
            address: _mailerConfig.SenderMail));

        message.To.Add(new MailboxAddress(item.Email, item.Email));
        message.Subject = item.Subject;

        message.Body = new TextPart("plain")
        {
            Text = item.Body
        };

        using var client = new SmtpClient();

        try
        {
            client.Connect(
                host: _mailerConfig.Host,
                port: _mailerConfig.Port,
                useSsl: _mailerConfig.UseSsl);

            // Note: only needed if the SMTP server requires authentication
            await client.AuthenticateAsync(
                userName: _mailerConfig.UserName,
                password: _mailerConfig.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Send email to {item.Email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during send email to {item.Email}");
        }
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<EmailEvent>();
        await handle.ResumeAsync(this);
    }
}
