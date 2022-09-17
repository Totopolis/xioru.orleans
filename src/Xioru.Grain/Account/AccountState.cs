namespace Xioru.Grain.Account;

public class AccountState
{
    public string AccountId { get; set; } = default!;

    public bool IsConfirmed { get; set; } = false;

    public string ConfirmCode { get; set; } = default!;

    public DateTime LastTouch { get; set; } = DateTime.MinValue;

    public string BillingEmail { get; set; } = default!;

    public string Password { get; set; } = default!;

    public HashSet<Guid> AccessibleProjects { get; set; } = new();
}
