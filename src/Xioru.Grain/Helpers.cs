using Orleans;
using Orleans.Streams;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.Mailer;

namespace Xioru.Grain;

public static class Helpers
{
    public static async Task<IAsyncStream<T>> GetStreamAndSingleSubscribe<T>(
        this IStreamProvider provider,
        Guid streamId,
        string streamNamespace,
        IAsyncObserver<T> observer)
    {
        var stream = provider.GetStream<T>(
            streamId,
            streamNamespace);

        var handles = await stream.GetAllSubscriptionHandles();

        if (handles == null || !handles.Any())
        {
            await stream.SubscribeAsync(observer);
        }
        else
        {
            await handles.First().ResumeAsync(observer);
        }

        return stream;
    }

    public static async Task UnSubscribeFromAll<T>(
        this IStreamProvider provider,
        Guid streamId,
        string streamNamespace)
    {
        var stream = provider.GetStream<T>(
            streamId,
            streamNamespace);

        var handles = await stream.GetAllSubscriptionHandles();

        foreach (var it in handles)
        {
            await it.UnsubscribeAsync();
        }
    }

    public static async Task SendEmail(
        this IGrainFactory factory,
        string email,
        string body,
        string subject = "")
    {
        var mailer = factory.GetGrain<IMailerGrain>(GrainConstants.MailerStreamId);
        await mailer.Send(email, body, subject);
    }
}
