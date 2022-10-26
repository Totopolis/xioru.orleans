namespace Xioru.Grain.Contracts.Mailer;

public class EmailEvent
{
    public string Email { get; set; } = default!;

    public string Subject { get; set; } = default!;

    public string Body { get; set; } = default!;
}
