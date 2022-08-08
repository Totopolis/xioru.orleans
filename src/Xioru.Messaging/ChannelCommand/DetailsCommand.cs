using Newtonsoft.Json;
using Orleans;
using System.CommandLine;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.ChannelCommand
{
    public partial class DetailsCommand : AbstractChannelCommand
    {
        private readonly Argument<string> _nameArgument =
            new Argument<string>("name", "unique object name");

        public DetailsCommand(IGrainFactory factory) : base(factory)
        {
        }

        public override Command Command => new Command(
            "details", "display object internals")
        {
            _nameArgument
        };

        protected override async Task<CommandResult> ExecuteInternal(
            ChannelCommandContext context)
        {
            var objName = context.Result.GetValueForArgument(_nameArgument);

            var grainDetails = await CheckGrain(objName);
            var serializedDetails = JsonConvert.SerializeObject(grainDetails, Formatting.Indented);
            var result = new FormattedString($"{objName} ", StringFormatting.Bold)
                .Append("details:\n\n");
            result.Append(serializedDetails);

            return CommandResult.Success(result);

        }
    }
}
