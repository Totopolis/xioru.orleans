﻿namespace Xioru.Grain.Contracts.Messages;

[GenerateSerializer]
public abstract record class GrainUpdatedEvent(DateTime UpdatedUtc) : GrainEvent;
