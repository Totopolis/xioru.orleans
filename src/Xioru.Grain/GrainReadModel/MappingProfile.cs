using AutoMapper;
using Ofg.Core.Contracts.Techno.Events;
using Xioru.Grain.Contracts.Messages;
using Xioru.Grain.Contracts.Project.Events;

namespace Xioru.Grain.GrainReadModel
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GrainCreatedEvent, GrainInfo>()
                .ForMember(dest => dest.GrainId, opt => opt.MapFrom(src => src.Metadata!.GrainId))
                .ForMember(dest => dest.GrainType, opt => opt.MapFrom(src => src.Metadata!.GrainType))
                .ForMember(dest => dest.GrainName, opt => opt.MapFrom(src => src.Metadata!.GrainName))
                .IncludeAllDerived();

            CreateMap<TechnoCreatedEvent, TechnoGrainInfo>();
            CreateMap<TechnoUpdatedEvent, TechnoGrainInfo>();
        }
    }
}
