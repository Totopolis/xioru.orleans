using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Runtime;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Account;
using Xioru.Grain.Contracts.ClusterRegistry;
using Xioru.Grain.Contracts.Config;
using Xioru.Grain.Contracts.Exception;

namespace Xioru.Grain.Account;

public partial class AccountGrain : Orleans.Grain, IAccountGrain
{
    private readonly IPersistentState<AccountState> _state;
    private readonly ILogger<AccountGrain> _log;
    private readonly IGrainFactory _grainFactory;

    private readonly MailerSection _mailerConfig;
    private readonly AuthSection _authConfig;
    private readonly ApiSection _apiConfig;

    private AccountState State => _state.State;

    public AccountGrain(
        [PersistentState("State", GrainConstants.StateStorageName)] IPersistentState<AccountState> state,
        ILogger<AccountGrain> log,
        IGrainFactory grainFactory,
        IOptions<AuthSection> authOptions,
        IOptions<MailerSection> mailerOptions,
        IOptions<ApiSection> apiOptions)
    {
        _state = state;
        _log = log;
        _grainFactory = grainFactory;

        _mailerConfig = mailerOptions.Value;
        _authConfig = authOptions.Value;
        _apiConfig = apiOptions.Value;
    }

    public string AccountId => this.GetPrimaryKeyString();

    // exec from /create user
    public async Task InviteToProject(Guid projectId)
    {
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
        if (!_state.RecordExists)
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
        sb.Append($"{_apiConfig.HostName}/confirm?account=");
        sb.Append(HttpUtility.UrlEncode(email));
        sb.AppendLine($"&code={State.ConfirmCode}");
        sb.AppendLine($"2. Second way - type code: {State.ConfirmCode}");

        sb.AppendLine();

        sb.AppendLine("Or ignore this email.");

        await _grainFactory.SendEmail(
            email: email,
            body: sb.ToString(),
            subject: "Need confirm email");

        State.AccountId = AccountId;
        State.IsConfirmed = false;
        await _state.WriteStateAsync();
    }

    public async Task<Token> Confirm(string confirmCode, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(confirmCode);
        ArgumentException.ThrowIfNullOrEmpty(password);

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
        CheckConfirmed();

        throw new NotImplementedException();
    }

    public Task SetBillingEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountProjection> GetProjection()
    {
        CheckConfirmed();

        var clusterRegistry = _grainFactory.GetGrain<IClusterRegistryGrain>(GrainConstants.ClusterStreamId);
        var projectList = await clusterRegistry.GetProjectsByFilter(string.Empty);

        return new AccountProjection(
            AccountId: AccountId,
            AccessibleProjects: projectList.Where(x => State.AccessibleProjects.Contains(x.Id)).ToArray());
    }

    public async Task ForceCreate(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        State.IsConfirmed = true;
        State.ConfirmCode = string.Empty;
        State.LastTouch = DateTime.MinValue;
        State.BillingEmail = AccountId;
        State.Password = password;

        await _state.WriteStateAsync();
    }

    private void CheckConfirmed()
    {
        if (!_state.RecordExists || !State.IsConfirmed)
        {
            _log.LogError($"Account '{AccountId}' not confirmed");
            throw new AccountNotConfirmedException();
        }
    }
}
