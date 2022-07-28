using AutoMapper;
using Ofg.Core.Contracts.ReadModels;
using Ofg.Core.Contracts.Techno.Events;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Grain.Contracts.Messages;
using Xioru.Grain.GrainReadModel.State;

namespace Xioru.Grain.GrainReadModel
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ShouldUseConstructor = _ => true;
            CreateMap<GrainCreatedEvent, GrainDetailsDocument>()
                .ForMember(dest => dest.GrainId, opt => opt.MapFrom(src => src.Metadata!.GrainId))
                .ForMember(dest => dest.GrainType, opt => opt.MapFrom(src => src.Metadata!.GrainType))
                .ForMember(dest => dest.GrainName, opt => opt.MapFrom(src => src.Metadata!.GrainName))
                .IncludeAllDerived();

            CreateMap<TechnoCreatedEvent, TechnoGrainDetailsDocument>();
            CreateMap<TechnoUpdatedEvent, TechnoGrainDetailsDocument>();
            
            CreateMap<GrainDetailsDocument, GrainDetails>()
                .IncludeAllDerived();
            CreateMap<TechnoGrainDetailsDocument, TechnoGrainDetails>();
        }
    }
}
