using Orleans;
using System.Threading.Tasks;

namespace Xioru.Orleans.Tests.Foo
{
    public interface IFooGrain : IGrainWithGuidKey
    {
        Task Create(CreateFooCommand createCommand);

        Task Update(UpdateFooCommand updateCommand);

        Task Delete();

        Task<FooProjection> GetProjection();
    }
}
