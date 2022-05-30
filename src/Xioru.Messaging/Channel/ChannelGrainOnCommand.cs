using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using Xioru.Messaging.Contracts.Channel;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.CommandExecutor;

namespace Xioru.Messaging.Channel
{
    public partial class ChannelGrain 
    {
        public async Task<CommandResult> ExecuteCommand(string command, bool isSupervisor = false)
        {
            try
            {
                var executor = _grainFactory.GetGrain<ICommandExecutor>(Guid.Empty);
                var result = await executor.Execute(
                    State.ProjectId,
                    this.GetPrimaryKey(),
                    isSupervisor,
                    command);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError("Internal error", ex);
                var errorMessage = $"Internal error: {ex.Message}";
                return CommandResult.InternalError(errorMessage);
            }
        }

        public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
        {
            var handle = handleFactory.Create<ChannelIncomingMessage>();
            await handle.ResumeAsync(this);
        }

        public async Task OnNextAsync(IList<SequentialItem<ChannelIncomingMessage>> items)
        {
            foreach (var it in items)
            {
                if (it == null)
                {
                    continue;
                }

                _log.LogInformation($"Channel received message {it!.Item}");

                try
                {
                    var executor = _grainFactory.GetGrain<ICommandExecutor>(Guid.Empty);
                    var result = await executor.Execute(
                        State.ProjectId,
                        this.GetPrimaryKey(),
                        it.Item.IsSupervisor,
                        it.Item.Text);

                    if (!string.IsNullOrWhiteSpace(result.Message))
                    {
                        await SendMessage(result.Message);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: почему в бот не отправляется сообщение с ошибкой?
                    _log.LogError("Internal error", ex);
                    var message = $"Internal error: {ex.Message}";
                    await SendMessage(message);
                }
            }
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            _log.LogError($"Channel error at receive channel-incoming: {ex.Message}");

            return Task.CompletedTask;
        }
    }
}
