using AutoMapper;
using Xioru.Grain.Contracts.User;
using Xioru.Grain.User;

namespace Xioru.Grain.ApiKey
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserState, UserProjection>();
            CreateMap<CreateUserCommandModel, UserState>();
            CreateMap<UpdateUserCommandModel, UserState>();
        }
    }
}
