using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Orleans;
using Orleans.Runtime;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web;
using Xioru.Grain.Contracts.Account;
using Xioru.Grain.Contracts.Config;

namespace Xioru.Grain.Account
{
    public partial class AccountGrain : Orleans.Grain, IAccountGrain
    {
        // TODO: move to appsettings
        public const string HostName = "http://api.domain.com";
        public const string MailerBox = "no-reply@domain.com";

        private readonly IPersistentState<AccountState> _state;
        private readonly ILogger<AccountGrain> _log;
        private readonly IGrainFactory _grainFactory;

        private readonly MailerSection _mailerConfig;
        private readonly AuthSection _authConfig;

        private AccountState State => _state.State;

        public AccountGrain(
            [PersistentState("state", "iotStore")] IPersistentState<AccountState> state,
            ILogger<AccountGrain> log,
            IGrainFactory grainFactory,
            IConfiguration config)
        {
            _state = state;
            _log = log;
            _grainFactory = grainFactory;

            _mailerConfig = config
                .GetSection(MailerSection.SectionName)
                .Get<MailerSection>();

            _authConfig = config
                .GetSection(AuthSection.SectionName)
                .Get<AuthSection>();
        }

        public string AccountId => this.GetPrimaryKeyString();

        // exec from /create user
        public async Task InviteToProject(Guid projectId)
        {
            if (State.AccessibleProjects == null)
            {
                State.AccessibleProjects = new();
            }

            State.AccessibleProjects.Add(projectId);
            await _state.WriteStateAsync();

            if (!State.IsConfirmed)
            {
                await Hello();
            }
        }

        // exec from /delete user
        public async Task RemoveFromProject(Guid projectId)
        {
            if (!_state.RecordExists ||
                State.AccessibleProjects == null)
            {
                return;
            }

            State.AccessibleProjects.Remove(projectId);
            await _state.WriteStateAsync();
        }

        public async Task Hello()
        {
            var email = AccountId;

            if (State.IsConfirmed)
            {
                var msg = $"Account '{email}' already confirmed";
                _log.LogError(msg);
                throw new InvalidOperationException(msg);
            }

            if (!new EmailAddressAttribute().IsValid(email))
            {
                throw new ArgumentException("Bad email");
            }

            var rnd = new Random();

            State.ConfirmCode = Enumerable
                .Range(0, 6)
                .Select(x => rnd.Next(0, 9))
                .Aggregate(string.Empty, (s, i) => s + i.ToString());

            var sb = new StringBuilder();
            sb.AppendLine("We recently received a request to create an account.");
            sb.AppendLine("To verify that you made this request, we're sending this confirmation email.");
            sb.Append("1. First way - click link: ");
            sb.Append($"{HostName}/confirm?account=");
            sb.Append(HttpUtility.UrlEncode(email));
            sb.AppendLine($"&code={State.ConfirmCode}");
            sb.AppendLine($"2. Second way - type code: {State.ConfirmCode}");

            sb.AppendLine();

            sb.AppendLine("Or ignore this email.");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(MailerBox, MailerBox));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "Need confirm email";

            message.Body = new TextPart("plain")
            {
                Text = sb.ToString()
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

                _log.LogInformation($"Send confirmation email to {email}");

                State.AccountId = AccountId;
                State.IsConfirmed = false;
                await _state.WriteStateAsync();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error during send email to {email}");
            }
        }

        public async Task<Token> Confirm(string confirmCode, string password)
        {
            if (State.ConfirmCode != confirmCode)
            {
                throw new ArgumentException("Bad confirmation code");
            }

            // TODO: check password policy

            State.IsConfirmed = true;
            State.ConfirmCode = string.Empty;
            State.LastTouch = DateTime.UtcNow;
            State.BillingEmail = AccountId;
            State.Password = password;

            await _state.WriteStateAsync();

            return await Login(password);
        }

        public Task ResetPassword()
        {
            if (!_state.RecordExists ||
                !State.IsConfirmed)
            {
                var msg = $"Account '{AccountId}' not confirmed";
                _log.LogError(msg);
                throw new InvalidOperationException(msg);
            }

            throw new NotImplementedException();
        }

        public Task SetBillingEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<AccountProjection> GetProjection()
        {
            if (!_state.RecordExists ||
                !State.IsConfirmed)
            {
                var msg = $"Account '{AccountId}' not confirmed";
                _log.LogError(msg);
                throw new InvalidOperationException(msg);
            }

            return Task.FromResult(new AccountProjection(
                AccountId: AccountId,
                AccessibleProjects: State.AccessibleProjects == null ?
                    new Guid[0] :
                    State.AccessibleProjects.ToArray()));
        }
    }
}
