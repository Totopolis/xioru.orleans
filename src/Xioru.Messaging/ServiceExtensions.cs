using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Xioru.Messaging.Channel;
using Xioru.Messaging.ChannelCommand;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Config;
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
            services.AddTransient<IMessengerRepository, MongoMessengerRepository>();

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

            //common channel commands
            services.AddTransient<IChannelCommand, ListCommand>();
            services.AddTransient<IChannelCommand, DetailsCommand>();

            // validators
            services.AddValidatorsFromAssemblyContaining<ChannelGrain>();

            // mappers
            services.AddAutoMapper(typeof(ChannelGrain));

            // register messengers
            var messengersSection = config.GetSection(BotsConfigSection.SectionName);
            if (messengersSection.Exists())
            {
                var messengersConfigs = messengersSection.Get<BotsConfigSection>()?.Configs;

                if (messengersConfigs != null)
                {
                    services.AddOptions<BotsConfigSection>()
                        .BindConfiguration(BotsConfigSection.SectionName);

                    services.RegisterMessengerIfEnabled<IDiscordMessengerGrain>(
                        messengersConfigs, MessengerType.Discord);

                    services.RegisterMessengerIfEnabled<ITelegramMessengerGrain>(
                        messengersConfigs, MessengerType.Telegram);
                }
            }

            return services;
        }

        private static void RegisterMessengerIfEnabled<T>(
            this IServiceCollection services,
            Dictionary<MessengerType, MessengerSection> configs,
            MessengerType messengerType) where T : IMessengerGrain
        {
            if (configs.TryGetValue(messengerType, out var messenger)
                && messenger != null
                && messenger.Enable)
            {
                services.AddTransient<IMessengerGrain>(
                    sb => sb.GetService<IGrainFactory>()!.GetGrain<T>(Guid.Empty));
            }
        }
    }
}
