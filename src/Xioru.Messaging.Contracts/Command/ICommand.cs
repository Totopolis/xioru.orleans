namespace Xioru.Messaging.Contracts.Command
{
    public interface ICommand
    {
        System.CommandLine.Command Command { get; }

        Task<CommandResult> Execute(CommandContext context);
    }
}
