namespace Xioru.Grain.Contracts.ProjectRegistry;

public interface IProjectRegistryWriterGrain : IGrainWithGuidKey
{
    Task OnGrainCreated(string name, Guid guid, string typeName);

    Task OnGrainDeleted(string name, Guid guid, string typeName);
}
