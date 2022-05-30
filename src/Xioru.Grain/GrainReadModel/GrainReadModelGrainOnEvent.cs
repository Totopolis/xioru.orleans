using MongoDB.Driver;
using Xioru.Grain.Contracts;

namespace Xioru.Grain.GrainReadModel
{
    public partial class GrainReadModelGrain
    {
        private async Task OnCreateEvent(GrainMessage item)
        {
            var docToInsert = new GrainDocument
            {
                GrainType = item.GrainType,
                GrainId = item.GrainId,
                GrainName = item.GrainName
            };

            await _grainCollection.InsertOneAsync(docToInsert);
        }

        private async Task OnUpdateEvent(GrainMessage item)
        {
            var docToUpdate = new GrainDocument
            {
                GrainType = item.GrainType,
                GrainId = item.GrainId,
                GrainName = item.GrainName
            };

            await _grainCollection.ReplaceOneAsync(
                x => x.GrainId == item.GrainId,
                docToUpdate);
        }

        private async Task OnDeleteEvent(GrainMessage item)
        {
            await _grainCollection
                            .DeleteOneAsync(x => x.GrainId == item.GrainId);
        }
    }
}
