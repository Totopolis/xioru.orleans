using Orleans;

namespace Xioru.Grain.Contracts.User
{
    public interface IUserGrain : IGrainWithGuidKey
    {
        Task Create(CreateUserCommand createCommand);

        Task Update(UpdateUserCommand updateCommand);

        Task Delete();

        Task<UserProjection> GetProjection();
    }
}
