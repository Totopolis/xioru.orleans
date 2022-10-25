using Orleans;

namespace Xioru.Grain.Contracts.User;

public interface IUserGrain : IGrainWithGuidKey
{
    Task CreateAsync(CreateUserCommandModel createCommand);

    Task UpdateAsync(UpdateUserCommandModel updateCommand);

    Task DeleteAsync();

    Task<UserProjection> GetProjection();
}
