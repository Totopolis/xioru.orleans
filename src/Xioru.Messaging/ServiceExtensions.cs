using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xioru.Messaging.Channel;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;
using Xioru.Messaging.MessengerCommand;

namespace Xioru.Messaging
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMessagingServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddTransient<IMessengerRepository, MessengerRepository>();

            // register user messenger commands
            services.AddTransient<IMessengerCommand, UnameCommand>();
            services.AddTransient<IMessengerCommand, PwdCommand>();
            services.AddTransient<IMessengerCommand, CdCommand>();
            services.AddTransient<IMessengerCommand, InviteCommand>();
            services.AddTransient<IMessengerCommand, JoinCommand>();
            services.AddTransient<IMessengerCommand, StartCommand>();
            services.AddTransient<IMessengerCommand, MecCommand>();
            services.AddTransient<IMessengerCommand, LeaveCommand>();

            // supervisor commands
            services.AddTransient<IMessengerCommand, SinviteCommand>();
            services.AddTransient<IMessengerCommand, SpwdCommand>();
            services.AddTransient<IMessengerCommand, ScdCommand>();

            // validators
            services.AddValidatorsFromAssemblyContaining<ChannelGrain>();

            // mappers
            services.AddAutoMapper(typeof(ChannelGrain));

            return services;
        }
    }
}
