using Orleans;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger
{
    public abstract class BaseMessengerCommand : IMessengerCommand
    {
        protected readonly IGrainFactory _factory;

        public BaseMessengerCommand(
            IGrainFactory factory,
            string commandName,
            string subCommandName,
            int minArgumentsCount,
            int maxArgumentsCount,
            string usage)
        {
            _factory = factory;

            CommandName = commandName;
            SubCommandName = subCommandName;
            MinArgumentsCount = minArgumentsCount;
            MaxArgumentsCount = maxArgumentsCount;
            Usage = usage;
        }

        public string CommandName { get; init; }

        public string SubCommandName { get; init; }

        public bool IsSubCommandExists
            => !string.IsNullOrWhiteSpace(SubCommandName);

        public int MinArgumentsCount { get; init; }

        public int MaxArgumentsCount { get; init; }

        public string Usage { get; init; }

        // No exceptions
        public async Task<CommandResult> Execute(CommandContext context)
        {
            var ctx = context as MessengerCommandContext;

            try
            {
                if (ctx == null)
                {
                    throw new CommandInternalErrorException("Bad context");
                }

                if (context.Arguments.Length < MinArgumentsCount ||
                    context.Arguments.Length > MaxArgumentsCount)
                {
                    throw new CommandSyntaxErrorException($"Bad usage\nMust be: {Usage}");
                }

                return await ExecuteInternal(ctx);
            }
            catch (CommandInternalErrorException ex)
            {
                return CommandResult.InternalError(ex.Message);
            }
            catch (CommandLogicErrorException ex)
            {
                return CommandResult.LogicError(ex.Message);
            }
            catch (CommandSyntaxErrorException ex)
            {
                return CommandResult.SyntaxError(ex.Message);
            }
            catch (Exception ex)
            {
                return CommandResult.InternalError($"Unknown error: {ex.Message}");
            }
        }

        protected abstract Task<CommandResult> ExecuteInternal(MessengerCommandContext context);
    }
}
