namespace Xioru.Grain.Contracts.Account
{
    public class Token
    {
        public string AccessToken { get; set; } = default!;

        public long ExpiredAt { get; set; }

        public string RefreshToken { get; set; } = default!;
    }
}
