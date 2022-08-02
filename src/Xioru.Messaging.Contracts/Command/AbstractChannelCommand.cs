using Orleans;
using Xioru.Grain.Contracts.GrainReadModel;
using Xioru.Messaging.Contracts.Channel;

namespace Xioru.Messaging.Contracts.Command
{
    public abstract class AbstractChannelCommand : IChannelCommand
    {
        protected readonly IGrainFactory _factory;
        protected IGrainReadModelGrain _grainReadModel = default!;

        public AbstractChannelCommand(
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
            var ctx = context as ChannelCommandContext;

            try
            {
                if (ctx == null)
                {
                    throw new CommandInternalErrorException("Bad context");
                }

                if (ctx.ProjectId == Guid.Empty)
                {
                    throw new CommandInternalErrorException("No current project");
                }

                _grainReadModel = _factory.GetGrain<IGrainReadModelGrain>(ctx.ProjectId);

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

        protected abstract Task<CommandResult> ExecuteInternal(ChannelCommandContext context);

        protected async Task<GrainDetails> CheckGrain(
            string grainName,
            string? grainType = null)
        {
            if (string.IsNullOrWhiteSpace(grainName))
            {
                throw new CommandSyntaxErrorException("Empty grain name");
            }

            var grainDetails = await _grainReadModel.GetGrainByName(grainName);

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
    }
}
