namespace Xioru.Grain.Contracts.Account;

[GenerateSerializer]
public class Token
{
    [Id(0)]
    public string AccessToken { get; set; } = default!;

    [Id(1)]
    public long ExpiredAt { get; set; }

    [Id(2)]
    public string RefreshToken { get; set; } = default!;
}
