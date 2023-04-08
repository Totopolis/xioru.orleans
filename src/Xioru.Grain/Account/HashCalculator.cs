using HashDepot;
using System.Text;
using Xioru.Grain.Contracts.Account;

namespace Xioru.Grain.Account;

public class HashCalculator : IHashCalculator
{
    public byte[] Calculate(string data, string salt)
    {
        byte[] _hashSalt = new byte[16];

        if (!string.IsNullOrWhiteSpace(salt))
        {
            var saltLen = salt.Length < 16 ?
                salt.Length :
                16;

            var saltString = salt.Substring(0, saltLen);
            var saltBuf = Encoding.ASCII.GetBytes(saltString);

            Buffer.BlockCopy(
                src: saltBuf,
                srcOffset: 0,
                dst: _hashSalt,
                dstOffset: 0,
                count: saltLen);
        }

        var buffer = Encoding.UTF8.GetBytes(data);
        var hash = SipHash24.Hash64(buffer, _hashSalt);

        return BitConverter.GetBytes(hash);
    }
}
