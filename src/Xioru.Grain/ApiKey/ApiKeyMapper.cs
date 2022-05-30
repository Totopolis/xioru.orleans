using AutoMapper;
using Xioru.Grain.Contracts.ApiKey;

namespace Xioru.Grain.ApiKey
{
    public class ApiKeyMapper : Profile
    {
        public ApiKeyMapper()
        {
            CreateMap<ApiKeyState, ApiKeyProjection>();
        }
    }
}
