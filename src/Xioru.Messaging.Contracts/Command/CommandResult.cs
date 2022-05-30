namespace Xioru.Messaging.Contracts.Command
{
    public class CommandResult
    {
        public CommandResult(ResultKind kind, string message)
        {
            Kind = kind;
            Message = message;
        }

        public bool IsSuccess => Kind == ResultKind.Ok;

        public string Message { get; init; }

        public ResultKind Kind { get; init; }

        public static CommandResult SyntaxError(string message) =>
            new CommandResult(ResultKind.SyntaxError, message);

        public static CommandResult LogicError(string message) =>
            new CommandResult(ResultKind.LogicError, message);

        public static CommandResult InternalError(string message) =>
            new CommandResult(ResultKind.InternalError, message);

        public static CommandResult Success(string message) =>
            new CommandResult(ResultKind.Ok, message);

        public enum ResultKind
        {
            Ok,
            InternalError,
            LogicError,
            SyntaxError
        }
    }
}
