using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.Contracts.Messenger;

public interface IMessengerGrain : IGrainWithGuidKey
{
    Task StartAsync();

    Task SendDirectMessage(string chatId, FormattedString message);
}
