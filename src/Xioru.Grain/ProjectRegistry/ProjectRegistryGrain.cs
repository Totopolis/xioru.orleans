using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Xioru.Grain.Contracts;
using Xioru.Grain.Contracts.ProjectRegistry;

namespace Xioru.Grain.ProjectRegistry
{
    public class ProjectRegistryGrain : 
        IProjectRegistryGrain,
        IProjectRegistryWriterGrain
    {
        private readonly IPersistentState<ProjectRegistryState> _state;
        private readonly ILogger<ProjectRegistryGrain> _logger;
        public ProjectRegistryGrain(
            [PersistentState("State", GrainConstants.StateStorageName)]
            IPersistentState<ProjectRegistryState> state,
            ILogger<ProjectRegistryGrain> logger) 
        {
            _state = state;
            _logger= logger;
        }

        public async Task<GrainRegistryDetails?> GetGrainDetailsById(Guid id)
        {
            await Task.CompletedTask;
            return _state.State.RegistryDetails.FirstOrDefault(x => x.GrainId == id);
        }

        public async Task<GrainRegistryDetails?> GetGrainDetailsByName(string name)
        {
            await Task.CompletedTask;
            return _state.State.RegistryDetails.FirstOrDefault(x => x.GrainName == name);
        }

        public async Task<IReadOnlyCollection<GrainRegistryDetails>> GetGrains()
        {
            await Task.CompletedTask;
            return _state.State.RegistryDetails.ToList();
        }

        public async Task OnGrainCreated(string name, Guid guid, string typeName)
        {
            await Task.CompletedTask;
            _state.State.RegistryDetails.Add(new(name, typeName, guid));
            await _state.WriteStateAsync();
        }

        public async Task OnGrainDeleted(string name, Guid guid, string typeName)
        {
            await Task.CompletedTask;
            var countRemoved = _state.State.RegistryDetails.RemoveAll(
                x => x.GrainName == name
                && x.GrainId == guid
                && x.GrainType == typeName);
            await _state.WriteStateAsync();
            if (countRemoved != 1)
            {
                _logger.LogWarning($"Grain {name} had to be removed from registry, but removed {countRemoved}");
            }
        }
    }
}
