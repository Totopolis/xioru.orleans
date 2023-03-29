using System.Threading.Tasks;
using Xioru.Grain.Contracts.Account;
using Xioru.Grain.Contracts.Exception;
using Xioru.Orleans.Tests.Common;
using Xunit;

namespace Xioru.Orleans.Tests;

public class AccountTest : AbstractTest
{
    public const string _email = "user@domain.com";
    public const string _password = "testpassword";

    [Fact]
    public async Task ForceCreateAccount_InEmptyCluster_Success()
    {
        await PrepareAsync();

        var account = _factory.GetGrain<IAccountGrain>(_email);
        await account.ForceCreate(_password);
        var projection = await account.GetProjection();

        Assert.True(projection.IsConfirmed);
    }

    [Fact]
    public async Task Login_ForceCreatedAccount_Success()
    {
        await PrepareAsync();

        var account = _factory.GetGrain<IAccountGrain>(_email);
        await account.ForceCreate(_password);

        var token = await account.Login(_password);
        Assert.NotEmpty(token.AccessToken);
    }

    [Fact]
    public async Task Login_WrongPassword_AccountBadPasswordException()
    {
        await PrepareAsync();

        var account = _factory.GetGrain<IAccountGrain>(_email);
        await account.ForceCreate(_password);

        try
        {
            var token = await account.Login(_password + "_trash");
            Assert.Fail("Must be exception");
        }
        catch (AccountBadPasswordException)
        {
        }
    }
}
