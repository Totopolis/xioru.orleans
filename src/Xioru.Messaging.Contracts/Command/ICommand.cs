namespace Xioru.Messaging.Contracts.Command
{
    public interface ICommand
    {
        string Name { get; }

        string Description { get; }

        string GetHelp();

        Task<CommandResult> Execute(CommandContext context);
    }
}
