using Orleans;

namespace Xioru.Grain.Contracts.Account
{
    /// <summary>
    /// GrainId = email or phone number
    /// </summary>
    public interface IAccountGrain : IGrainWithStringKey
    {
        Task InviteToProject(Guid projectId);

        Task RemoveFromProject(Guid projectId);

        // If not confirmed - send email with url (accountId+confirmCode)
        Task Hello();

        // If success - user can set password
        Task<Token> Confirm(string confirmCode, string password);

        // If success generate new jwt
        Task<Token> Login(string password);

        Task<Token> RefreshToken(string refreshToken);

        // send email with confirmation code (+accountId+confirmCode)
        Task ResetPassword();

        Task SetBillingEmail(string email);

        Task<AccountProjection> GetProjection();

        /*Task AttachPhone(string phone);

        Task ConfirmPhone(string confirmCode);

        Task AttachEmail(string email);

        Task ConfirmEmail(string confirmCode);

        Task Enable2FA();

        Task Disable2FA();*/
    }
}
