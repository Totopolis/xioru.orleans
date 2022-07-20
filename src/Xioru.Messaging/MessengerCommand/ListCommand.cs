using ConsoleTables;
using Orleans;
using System.Text;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;
using Xioru.Messaging.Contracts.Messenger;
using Xioru.Messaging.Messenger;

namespace Xioru.Messaging.MessengerCommand
{
    public class ListCommand : BaseMessengerCommand
    {
        public ListCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "list",
            subCommandName: string.Empty,
            minArgumentsCount: 0,
            maxArgumentsCount: 0,
            usage: "/list")
        {
        }

        protected override async Task<CommandResult> ExecuteInternal(MessengerCommandContext context)
        {
            var readModel = _factory.GetGrain<IGrainReadModelGrain>(GrainConstants.ClusterStreamId);
            
            if(!context.Manager.TryGetCurrentChannel(context.ChatId, out var accessRecord))
            {

            }

            var grainDescriptions = await readModel.GetGrains();


            
            var fString = new FormattedString("> List of the platform objects", Formatting.Bold);
            fString.Append("```");

            var table = new ConsoleTable("Name", "Type");

            foreach (var grain in grainDescriptions)
            {
                table.AddRow(grain.GrainName, grain.GrainType.Replace("Grain", string.Empty));
            }

            fString.Append($"{table.ToMinimalString()}```");

            return CommandResult.Success(fString);
        }
    }
}
