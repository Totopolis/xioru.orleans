using AutoMapper;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Messages;
using Xioru.Grain.GrainReadModel.State;

namespace Xioru.Grain.GrainReadModel;

public class GrainReadModelMappingProfile : Profile
{
    public GrainReadModelMappingProfile()
    {
        ShouldUseConstructor = _ => true;
        CreateMap<GrainCreatedEvent, GrainDetailsDocument>()
            .ForMember(dest => dest.GrainId, opt => opt.MapFrom(src => src.Metadata!.GrainId))
            .ForMember(dest => dest.GrainType, opt => opt.MapFrom(src => src.Metadata!.GrainType))
            .ForMember(dest => dest.GrainName, opt => opt.MapFrom(src => src.Metadata!.GrainName))
            .IncludeAllDerived();

        CreateMap<GrainUpdatedEvent, GrainDetailsDocument>()
            .ForMember(dest => dest.GrainId, opt => opt.MapFrom(src => src.Metadata!.GrainId))
            .ForMember(dest => dest.GrainType, opt => opt.MapFrom(src => src.Metadata!.GrainType))
            .ForMember(dest => dest.GrainName, opt => opt.MapFrom(src => src.Metadata!.GrainName))
            .IncludeAllDerived();
        
        CreateMap<GrainDetailsDocument, GrainDetails>()
            .IncludeAllDerived();
    }
}
