using Newtonsoft.Json;
using Orleans;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.ChannelCommand
{
    public partial class DetailsCommand : AbstractChannelCommand
    {
        public const string UsageConst =
            "/details name";

        public DetailsCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "details",
            subCommandName: string.Empty,
            minArgumentsCount: 1,
            maxArgumentsCount: 1,
            usage: UsageConst)
        {
        }

        protected override async Task<CommandResult> ExecuteInternal(
            ChannelCommandContext context)
        {
            var objName = context.Arguments[0];

            var grainDetails = await CheckGrain(objName);
            var serializedDetails = JsonConvert.SerializeObject(grainDetails, Formatting.Indented);
            var result = new FormattedString($"{objName} ", StringFormatting.Bold)
                .Append("details:\n\n");
            result.Append(serializedDetails);

            return CommandResult.Success(result);

        }
    }
}
