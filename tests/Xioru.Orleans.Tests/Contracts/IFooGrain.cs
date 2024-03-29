﻿using Orleans;
using System.Threading.Tasks;

namespace Xioru.Orleans.Tests.Contracts;

public interface IFooGrain : IGrainWithGuidKey
{
    Task CreateAsync(CreateFooCommandModel createCommand);

    Task UpdateAsync(UpdateFooCommandModel updateCommand);

    Task DeleteAsync();

    Task<FooProjection> GetProjection();
}
