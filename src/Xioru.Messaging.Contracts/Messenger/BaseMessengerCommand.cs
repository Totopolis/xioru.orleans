using Orleans;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger
{
    public abstract class BaseMessengerCommand : IMessengerCommand
    {
        protected readonly IGrainFactory _factory;

        public BaseMessengerCommand(IGrainFactory factory)
        {
            _factory = factory;
        }

        public abstract System.CommandLine.Command Command { get; }

        // No exceptions
        public async Task<CommandResult> Execute(CommandContext context)
        {
            var ctx = context as MessengerCommandContext;
            if (ctx == null)
            {
                throw new CommandInternalErrorException("Bad context");
            }

            try
            {
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
