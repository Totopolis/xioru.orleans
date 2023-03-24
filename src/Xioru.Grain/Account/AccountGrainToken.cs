using HashDepot;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xioru.Grain.Contracts.Account;
using Xioru.Grain.Contracts.Exception;

namespace Xioru.Grain.Account;

public partial class AccountGrain
{
    // https://www.c-sharpcorner.com/article/jwt-validation-and-authorization-in-net-5-0/
    public Task<Token> Login(string password)
    {
        CheckConfirmed();
        
        if (!State.PasswordHash.SequenceEqual<byte>(CalculateHash(password)))
        {
            var msg = $"Bad password for account '{AccountId}'";
            _log.LogError(msg);
            throw new AccountBadPasswordException();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authConfig.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", AccountId) }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _authConfig.Issuer,
            Audience = _authConfig.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenStr = tokenHandler.WriteToken(token);

        return Task.FromResult(new Token
        {
            AccessToken = tokenStr
        });
    }

    public Task<Token> RefreshToken(string refreshToken)
    {
        CheckConfirmed();

        throw new NotImplementedException();
    }

    private byte[] CalculateHash(string password)
    {
        byte[] _hashSalt = new byte[16];

        if (!string.IsNullOrWhiteSpace(_authConfig.HashSalt))
        {
            var saltLen = _authConfig.HashSalt.Length < 16 ?
                _authConfig.HashSalt.Length :
                16;

            var salt = _authConfig.HashSalt.Substring(0, saltLen);
            var saltBuf = Encoding.ASCII.GetBytes(salt);

            Buffer.BlockCopy(
                src: saltBuf,
                srcOffset: 0,
                dst: _hashSalt,
                dstOffset: 0,
                count: saltLen);
        }

        var buffer = Encoding.UTF8.GetBytes(password);
        var hash = SipHash24.Hash64(buffer, _hashSalt);

        return BitConverter.GetBytes(hash);
    }
}
