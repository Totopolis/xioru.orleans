using System.Threading.Tasks;
using Xioru.Messaging.Contracts.Command;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Orleans.Tests.VirtualMessenger;

public interface IVirtualMessengerGrain : IMessengerGrain
{
    Task<CommandResult> ExecuteSupervisorCommand(string commandText);
}
