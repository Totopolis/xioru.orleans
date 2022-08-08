using AutoMapper;
using Xioru.Grain.Contracts.ApiKey;

namespace Xioru.Grain.ApiKey
{
    public class ApiKeyMapper : Profile
    {
        public ApiKeyMapper()
        {
            CreateMap<ApiKeyState, ApiKeyProjection>();
            CreateMap<CreateApiKeyCommandModel, ApiKeyState>()
                .ForMember(x => x.ApiKey, opt => opt.MapFrom(x => Guid.NewGuid()))
                .ForMember(x => x.Created, opt => opt.MapFrom(x => DateTime.UtcNow));
            CreateMap<UpdateApiKeyCommandModel, ApiKeyState>();
        }
    }
}
