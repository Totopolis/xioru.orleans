using ConsoleTables;
using Orleans;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.ChannelCommand
{
    public class ListCommand : AbstractChannelCommand
    {
        public ListCommand(IGrainFactory factory) : base(
            factory: factory,
            commandName: "list",
            subCommandName: string.Empty,
            minArgumentsCount: 0,
            maxArgumentsCount: 1,
            usage: "/list filter")
        {
        }

        protected override async Task<CommandResult> ExecuteInternal(
            ChannelCommandContext context)
        {
            if (context.ArgsCount > 0 && context.Arguments[0].Length < 3)
            {
                throw new CommandLogicErrorException(
                    "Filter must be at least 3 characters long");
            }

            var grainDetails = await _grainReadModel
                .GetGrains(context.Arguments.FirstOrDefault());

            if (grainDetails!.Count == 0)
            {
                return CommandResult.Success("No objects found");
            }

            var fString = new FormattedString("List of the platform objects", StringFormatting.BoxedLine);

            var table = new ConsoleTable("Name", "Type");

            foreach (var grain in grainDetails)
            {
                var grainTypeShortened = grain.GrainType.EndsWith("Grain")
                    ? grain.GrainType.Substring(0, grain.GrainType.Length - "Grain".Length)
                    : grain.GrainType;
                table.AddRow(grain.GrainName, grainTypeShortened);
            }

            fString.Append($"{table.ToMinimalString()}", StringFormatting.Code);

            return CommandResult.Success(fString);
        }
    }
}
