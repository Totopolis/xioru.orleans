using AutoMapper;
using Xioru.Orleans.Tests.Contracts;

namespace Xioru.Orleans.Tests.Domain;

internal class FooMappingProfile : Profile
{
    public FooMappingProfile()
    {
        CreateMap<CreateFooCommandModel, FooState>();
        CreateMap<UpdateFooCommandModel, FooState>();
        CreateMap<FooState, FooProjection>();
    }
}
