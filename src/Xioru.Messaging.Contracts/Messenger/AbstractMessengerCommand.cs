using Orleans;
using System.CommandLine;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Messenger
{
    public abstract class AbstractMessengerCommand : IMessengerCommand
    {
        protected readonly IGrainFactory _factory;

        private System.CommandLine.Parsing.ParseResult _parseResult = default!;

        public AbstractMessengerCommand(IGrainFactory factory)
        {
            _factory = factory;
        }

        public virtual string Name => Command.Name;

        protected abstract System.CommandLine.Command Command { get; }

        public async Task<CommandResult> Execute(CommandContext context)
        {
            var ctx = context as MessengerCommandContext;
            if (ctx == null)
            {
                throw new CommandInternalErrorException("Bad context");
            }

            try
            {
                _parseResult = Command.Parse(context.CommandText);
                if (_parseResult.Errors.Any())
                {
                    var msg = string.Join(';', _parseResult.Errors.Select(x => x.Message));
                    return CommandResult.SyntaxError(msg);
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

        protected T GetArgumentValue<T>(Argument<T> argument)
        {
            return _parseResult.GetValueForArgument(argument);
        }

        protected T? GetOptionValue<T>(Option<T> option)
        {
            return _parseResult.GetValueForOption(option);
        }

        protected abstract Task<CommandResult> ExecuteInternal(MessengerCommandContext context);
    }
}
