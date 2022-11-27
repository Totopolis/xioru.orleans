using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mongo2Go;
using MongoDB.Driver;
using Orleans;
using Orleans.TestingHost;
using System;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Messaging;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Orleans.Tests.Domain;
using Xioru.Orleans.Tests.VirtualMessenger;

namespace Xioru.Orleans.Tests.Common;

public class HostConfigurator : IHostConfigurator
{
    private static readonly MongoDbRunner _mongoRunner;
    private static readonly MongoClient _mongoClient;

    static HostConfigurator()
    {
        _mongoRunner = MongoDbRunner.Start();
        _mongoClient = new MongoClient(_mongoRunner.ConnectionString);
    }

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

            var db = _mongoClient.GetDatabase(
                $"IntegrationTest_{Guid.NewGuid().ToString("N")}");
            services.AddSingleton<IMongoDatabase>(db);

            services.AddValidatorsFromAssemblyContaining<FooGrain>();
            services.AddAutoMapper(typeof(FooGrain));
        });
    }
}
