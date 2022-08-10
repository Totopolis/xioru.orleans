using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using Orleans;
using Orleans.Hosting;
using Orleans.TestingHost;
using System;
using Xioru.Grain;
using Xioru.Grain.Contracts;
using Xioru.Messaging;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Messenger;
using Xioru.Orleans.Tests.Domain;

namespace Xioru.Orleans.Tests.Common
{
    public class ClusterConfigurator : ISiloConfigurator
    {
        private static readonly MongoDbRunner _mongoRunner;
        private static readonly MongoClient _mongoClient;

        static ClusterConfigurator()
        {
            _mongoRunner = MongoDbRunner.Start();
            _mongoClient = new MongoClient(_mongoRunner.ConnectionString);
        }

        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.ConfigureServices((hbc, services) =>
            {
                services
                   .AddGrainServices(hbc.Configuration)
                   .AddMessagingServices(hbc.Configuration);

                services.AddTransient<IChannelCommand, UpsertFooCommand>();

                var db = _mongoClient.GetDatabase(
                    $"IntegrationTest_{Guid.NewGuid().ToString("N")}");
                services.AddSingleton<IMongoDatabase>(db);

                services.AddValidatorsFromAssemblyContaining<FooGrain>();
                services.AddAutoMapper(typeof(FooGrain));
            });

            siloBuilder.AddSimpleMessageStreamProvider(GrainConstants.StreamProviderName)
                .AddMemoryGrainStorage("PubSubStore")
                .AddMemoryGrainStorage(GrainConstants.StateStorageName)
                .UseInMemoryReminderService();

            siloBuilder.ConfigureApplicationParts(parts => parts
                .AddApplicationPart(typeof(FooGrain).Assembly).WithReferences()
                .AddApplicationPart(typeof(MessengerGrain).Assembly).WithReferences());
        }
    }
}
