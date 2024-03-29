﻿using Xioru.Grain.Contracts.Messages;

namespace Xioru.Grain.Contracts.Project.Events;

[GenerateSerializer]
public record ProjectCreatedEvent(DateTime CreatedUtc) : GrainCreatedEvent(CreatedUtc);
