namespace Xioru.Messaging.Contracts.Command
{
    public interface ICommand
    {
        string CommandName { get; }

        string SubCommandName { get; }

        bool IsSubCommandExists { get; }

        int MinArgumentsCount { get; }

        int MaxArgumentsCount { get; }

        string Usage { get; }

        Task<CommandResult> Execute(CommandContext context);
    }
}
