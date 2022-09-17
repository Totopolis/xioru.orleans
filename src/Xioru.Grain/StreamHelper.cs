using Orleans.Streams;

namespace Xioru.Grain;

public static class StreamHelper
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
}
