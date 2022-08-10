namespace Xioru.Messaging.Contracts.Command
{
    public interface ICommand
    {
        string Name { get; }

        Task<CommandResult> Execute(CommandContext context);
    }
}
