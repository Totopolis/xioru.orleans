using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ClusterRegistry;

namespace Xioru.Grain.ProjectRegistry
{
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

        public async Task<bool> OnProjectCreatedAsync(Guid projectId, string name)
        {
            await Task.CompletedTask;
            if(_state.State.RegistryDetails.Any(x => x.Id == projectId)
                || _state.State.RegistryDetails.Any(x => x.Name == name))
            {
                _logger.LogInformation($"Project {name} cannot be created, it already exists");
                return false;
            }
            
            _state.State.RegistryDetails.Add(new(name, projectId));
            return true;
        }

        public async Task<bool> OnProjectDeletedAsync(Guid projectId, string name)
        {
            await Task.CompletedTask;
            var countRemoved = _state.State.RegistryDetails.RemoveAll(
                x => x.Name == name
                && x.Id == projectId);
            if (countRemoved != 1)
            {
                _logger.LogWarning($"Project {name} had to be removed from the registry, but removed {countRemoved}");
                return false;
            }

            return true;
        }
    }
}
