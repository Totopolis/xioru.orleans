using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ClusterRegistry;

namespace Xioru.Grain.ClusterRegistry;

public class ClusterRegistryGrain : 
    IClusterRegistryGrain,
    IClusterRegistryWriterGrain
{
    private readonly IPersistentState<ClusterRegistryState> _state;
    private readonly ILogger<ClusterRegistryGrain> _logger;

    public ClusterRegistryGrain(
        [PersistentState("State", GrainConstants.StateStorageName)]
        IPersistentState<ClusterRegistryState> state,
        ILogger<ClusterRegistryGrain> logger) 
    {
        _state = state;
        _logger= logger;
    }

    public async Task<Guid?> GetProjectIdByNameOrDefaultAsync(string projectName)
    {
        await Task.CompletedTask;
        return _state.State.RegistryDetails.FirstOrDefault(x => x.Name == projectName)?.Id 
            ?? null;
    }

    public async Task<string?> GetProjectNameByIdOrDefaultAsync(Guid projectId)
    {
        await Task.CompletedTask;
        return _state.State.RegistryDetails.FirstOrDefault(x => x.Id == projectId)?.Name
            ?? default;
    }

    public async Task<ProjectDescription[]> GetProjectsByFilter(string projectNameFilter)
    {
        await Task.CompletedTask;

        var result = new ProjectDescription[0];

        if (_state.State.RegistryDetails != null)
        {
            result = _state.State.RegistryDetails
                .Select(x => new ProjectDescription(
                    Id: x.Id,
                    Name: x.Name))
                .ToArray();

            if (!string.IsNullOrWhiteSpace(projectNameFilter))
            {
                result = result
                    .Where(x => x.Name.Contains(projectNameFilter))
                    .ToArray();
            }
        }

        return result;
    }

    public async Task<bool> OnProjectCreatedAsync(Guid projectId, string name)
    {
        if(_state.State.RegistryDetails.Any(x => x.Id == projectId)
            || _state.State.RegistryDetails.Any(x => x.Name == name))
        {
            _logger.LogInformation($"Project {name} cannot be created, it already exists");
            return false;
        }
        
        _state.State.RegistryDetails.Add(new(name, projectId));
        await _state.WriteStateAsync();

        return true;
    }

    public async Task<bool> OnProjectDeletedAsync(Guid projectId, string name)
    {
        var countRemoved = _state.State.RegistryDetails.RemoveAll(
            x => x.Name == name
            && x.Id == projectId);

        await _state.WriteStateAsync();
        if (countRemoved != 1)
        {
            _logger.LogWarning($"Project {name} had to be removed from the registry, but removed {countRemoved}");
            return false;
        }

        return true;
    }
}
