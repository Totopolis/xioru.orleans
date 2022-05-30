using AutoMapper;
using Xioru.Grain.Contracts.Project;

namespace Xioru.Grain.Project
{
    public class ProjectMapper : Profile
    {
        public ProjectMapper()
        {
            CreateMap<CreateProjectCommand, ProjectState>();
        }
    }
}
