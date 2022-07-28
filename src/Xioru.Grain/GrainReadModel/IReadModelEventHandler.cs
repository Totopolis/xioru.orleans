using Orleans.Streams;
using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.GrainReadModel
{
    public interface IReadModelEventHandler :
        IAsyncObserver<GrainCreatedEvent>,
        IAsyncObserver<GrainUpdatedEvent>,
        IAsyncObserver<GrainDeletedEvent>
    {
        void SetCollectionName(string collectionName);
    }
}
