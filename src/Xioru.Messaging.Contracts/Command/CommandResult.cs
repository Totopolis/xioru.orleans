using Xioru.Messaging.Contracts.Formatting;

namespace Xioru.Messaging.Contracts.Command;

public class CommandResult
{
    public CommandResult(ResultKind kind, FormattedString message)
    {
        Kind = kind;
        Message = message;
    }

    public bool IsSuccess => Kind == ResultKind.Ok;

    public FormattedString Message { get; init; }

    public ResultKind Kind { get; init; }

    public static CommandResult SyntaxError(FormattedString message) =>
        new CommandResult(ResultKind.SyntaxError, message);

    public static CommandResult LogicError(FormattedString message) =>
        new CommandResult(ResultKind.LogicError, message);

    public static CommandResult InternalError(FormattedString message) =>
        new CommandResult(ResultKind.InternalError, message);

    public static CommandResult Success(FormattedString message) =>
        new CommandResult(ResultKind.Ok, message);

    public enum ResultKind
    {
        Ok,
        InternalError,
        LogicError,
        SyntaxError
    }
}
