using Orleans;
using System;
using System.CommandLine;
using System.Threading.Tasks;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Orleans.Tests.Contracts;

namespace Xioru.Orleans.Tests.Domain
{
    public class UpsertFooCommand : AbstractChannelCommand
    {
        // requred argument
        private readonly Argument<string> _nameArgument = new Argument<string>(
                name: "name",
                description: "foo name");

        // optional
        private readonly Option<string> _dataOption = new Option<string>(
                name: "data",
                description: "foo data",
                getDefaultValue: () => "nodata");

        public UpsertFooCommand(IGrainFactory factory) : base(factory)
        {
        }

        public override Command Command => new Command(
            "upsert", "upsert foo instance")
        {
            _nameArgument,
            _dataOption
        };

        protected override async Task<CommandResult> ExecuteInternal(
            ChannelCommandContext context)
        {
            var name = context.GetArgumentValue(_nameArgument);
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CommandSyntaxErrorException("Bad foo name");
            }

            var data = context.GetOptionValue(_dataOption)!;

            var details = await _grainReadModel.GetGrainDetailsByName(name);
            if (details == null)
            {
                var grain = _factory.GetGrain<IFooGrain>(Guid.NewGuid());

                await grain.CreateAsync(new CreateFooCommandModel(
                    ProjectId: context.ProjectId,
                    Name: name,
                    DisplayName: name,
                    Description: string.Empty,
                    Tags: Array.Empty<string>(),
                    FooData: data));
            }
            else
            {
                var grain = _factory.GetGrain<IFooGrain>(details.GrainId);

                await grain.UpdateAsync(new UpdateFooCommandModel(
                    DisplayName: name,
                    Description: string.Empty,
                    Tags: Array.Empty<string>(),
                    FooData: data));
            }

            return CommandResult.Success("ok");
        }
    }
}
