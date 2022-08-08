using Orleans;
using System.CommandLine;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class MecCommand : BaseMessengerCommand
    {
        public MecCommand(IGrainFactory factory) : base(factory)
        {
        }

        public override Command Command => new Command(
            "mec", "display my channel id");

        protected override Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            var fString = new FormattedString("ChannelId: ", StringFormatting.Bold);
            fString.Append(context.ChatId);

            return Task.FromResult(CommandResult.Success(fString));
        }
    }
}
