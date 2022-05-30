using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xioru.Grain.Project;

namespace Xioru.Grain
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddGrainServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            // validators
            services.AddValidatorsFromAssemblyContaining<ProjectGrain>();

            // mappers
            services.AddAutoMapper(typeof(ProjectGrain));

            services.AddTransient<IMemoryCache, MemoryCache>();

            return services;
        }
    }
}
