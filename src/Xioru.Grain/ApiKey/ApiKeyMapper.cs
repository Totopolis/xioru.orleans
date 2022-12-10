using AutoMapper;
using Xioru.Grain.Contracts.ApiKey;

namespace Xioru.Grain.ApiKey;

public class ApiKeyMapper : Profile
{
    public ApiKeyMapper()
    {
        CreateMap<ApiKeyState, ApiKeyProjection>();
        CreateMap<CreateApiKeyCommandModel, ApiKeyState>()
            .ForMember(x => x.ApiKey, opt => opt.MapFrom(x => Guid.NewGuid()));
        CreateMap<UpdateApiKeyCommandModel, ApiKeyState>();
    }
}
