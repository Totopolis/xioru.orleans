using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xioru.Grain.Account;
using Xioru.Grain.Contracts.Account;
using Xioru.Grain.Contracts.Config;
using Xioru.Grain.Project;

namespace Xioru.Grain;

public static class ServiceExtensions
{
    public static IServiceCollection AddGrainServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        // register default hash calculator
        services.AddSingleton<IHashCalculator, HashCalculator>();

        // validators
        services.AddValidatorsFromAssemblyContaining<ProjectGrain>();

        // mappers
        services.AddAutoMapper(typeof(ProjectGrain));

        services.AddTransient<IMemoryCache, MemoryCache>();

        // register options
        services.AddOptions<AuthSection>()
            .BindConfiguration(AuthSection.SectionName);

        services.AddOptions<MailerSection>()
            .BindConfiguration(MailerSection.SectionName);

        services.AddOptions<ApiSection>()
            .BindConfiguration(ApiSection.SectionName);

        return services;
    }
}
