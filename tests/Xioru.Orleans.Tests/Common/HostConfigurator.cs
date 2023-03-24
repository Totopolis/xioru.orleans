using EphemeralMongo;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Orleans;
using Orleans.TestingHost;
using System;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Config;
using Xioru.Messaging;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Orleans.Tests.Domain;
using Xioru.Orleans.Tests.VirtualMessenger;

namespace Xioru.Orleans.Tests.Common;

public class HostConfigurator : IHostConfigurator
{
    public void Configure(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((hbc, services) =>
        {
            services
               .AddGrainServices(hbc.Configuration)
               .AddMessagingServices(hbc.Configuration);

            // for MessagingStartupTask
            services.AddTransient<IMessengerGrain>(
                sb => sb.GetService<IGrainFactory>()!.GetGrain<IVirtualMessengerGrain>(Guid.Empty));

            services.AddTransient<IChannelCommand, UpsertFooCommand>();
            services.AddTransient<IVersionProvider, VersionProvider>();

            // get proxy-runner (provider increase internal useCounter)
            // proxy will be disposed by DI when the silos goes to down
            services.AddSingleton<IMongoRunner>(sp => MongoRunnerProvider.Get());

            services.AddSingleton<IMongoClient>(sp =>
            {
                var runner = sp.GetRequiredService<IMongoRunner>();
                return new MongoClient(runner.ConnectionString);
            });

            var dbName = $"IntegrationTest_{Guid.NewGuid().ToString("N")}";
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(dbName);
            });

            services.AddValidatorsFromAssemblyContaining<FooGrain>();
            services.AddAutoMapper(typeof(FooGrain));

            services.Configure<AuthSection>(section =>
            {
                section.Issuer = "xioru";
                section.Audience = "*";
                section.SecretKey = "liHLSfGebYvXGTDx0vWb53JhyUpnw6HvgRwOJ6h/hUs=";
                section.HashSalt = "mysalt";
            });
        });
    }
}
