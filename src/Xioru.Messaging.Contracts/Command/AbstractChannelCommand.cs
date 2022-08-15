using Orleans;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Messaging.Contracts.Channel;

namespace Xioru.Messaging.Contracts.Command
{
    public abstract class AbstractChannelCommand : IChannelCommand
    {
        protected readonly IGrainFactory _factory;
        protected IGrainReadModelGrain _grainReadModel = default!;

        private ParseResult _parseResult = default!;

        public AbstractChannelCommand(IGrainFactory factory)
        {
            _factory = factory;
        }

        public virtual string Name => Command.Name;

        public virtual string Description => Command.Description ?? string.Empty;

        public virtual string GetHelp()
        {
            using var textWriter = new StringWriter();
            var helpBuilder = new HelpBuilder(LocalizationResources.Instance);

            helpBuilder.Write(new HelpContext(helpBuilder, Command, textWriter));
            return textWriter.ToString();
        }

        protected abstract System.CommandLine.Command Command { get; }

        public async Task<CommandResult> Execute(CommandContext context)
        {
            var ctx = context as ChannelCommandContext;
            if (ctx == null)
            {
                throw new CommandInternalErrorException("Bad context");
            }

            try
            {
                if (ctx.ProjectId == Guid.Empty)
                {
                    throw new CommandInternalErrorException("No current project");
                }

                _parseResult = Command.Parse(context.CommandText);
                if (_parseResult.Errors.Any())
                {
                    var msg = string.Join(';', _parseResult.Errors.Select(x => x.Message));
                    return CommandResult.SyntaxError(msg);
                }

                _grainReadModel = _factory.GetGrain<IGrainReadModelGrain>(ctx.ProjectId);

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

        protected abstract Task<CommandResult> ExecuteInternal(ChannelCommandContext context);

        protected async Task<GrainDetails> CheckGrain(
            string grainName,
            string? grainType = null)
        {
            if (string.IsNullOrWhiteSpace(grainName))
            {
                throw new CommandSyntaxErrorException("Empty grain name");
            }

            var grainDetails = await _grainReadModel.GetGrainDetailsByName(grainName);

            if (grainDetails == null)
            {
                throw new CommandLogicErrorException($"Object {grainName} not found");
            }

            if (grainType != null && grainDetails.GrainType != grainType)
            {
                throw new CommandLogicErrorException($"Object {grainName} have bad type");
            }

            return grainDetails;
        }

        protected async Task<GrainDetails> CheckGrain<T>(string grainName) 
            where T: class, IGrainWithGuidKey 
        {
            if (string.IsNullOrWhiteSpace(grainName))
            {
                throw new CommandSyntaxErrorException("Empty grain name");
            }

            var grainDetails = await _grainReadModel.GetGrainDetailsByNameAndInterface<T>(grainName);

            if (grainDetails == null)
            {
                throw new CommandLogicErrorException($"Object {grainName} of required type not found");
            }

            return grainDetails;
        }
    }
}
