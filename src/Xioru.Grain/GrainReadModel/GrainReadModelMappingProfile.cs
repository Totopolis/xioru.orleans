using AutoMapper;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Messages;

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
            .ForMember(dest => dest.CreatedUtc, opt => opt.MapFrom(src => src.CreatedUtc))
            .ForMember(dest => dest.UpdatedUtc, opt => opt.MapFrom(src => src.CreatedUtc))
            .IncludeAllDerived();

        CreateMap<GrainUpdatedEvent, GrainDetailsDocument>()
            .ForMember(dest => dest.GrainId, opt => opt.MapFrom(src => src.Metadata!.GrainId))
            .ForMember(dest => dest.GrainType, opt => opt.MapFrom(src => src.Metadata!.GrainType))
            .ForMember(dest => dest.GrainName, opt => opt.MapFrom(src => src.Metadata!.GrainName))
            .ForMember(dest => dest.UpdatedUtc, opt => opt.MapFrom(src => src.UpdatedUtc))
            .IncludeAllDerived();
        
        CreateMap<GrainDetailsDocument, GrainDetails>()
            .IncludeAllDerived();
    }
}
