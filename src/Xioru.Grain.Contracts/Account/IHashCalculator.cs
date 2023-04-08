namespace Xioru.Grain.Contracts.Account;

public interface IHashCalculator
{
    byte[] Calculate(string data, string salt);
}
