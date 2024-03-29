﻿using Xioru.Grain.Contracts.AbstractGrain;
using Xioru.Messaging.Contracts.Messenger;

namespace Xioru.Messaging.Contracts.Channel;

[GenerateSerializer]
public record ChannelProjection(
    Guid Id,
    string Name,
    Guid ProjectId,
    string DisplayName,
    string Description,
    string[] Tags,
    DateTime CreatedUtc,
    DateTime UpdatedUtc,
    //
    MessengerType MessengerType,
    string ChatId,
    DateTime LastMessage) : AbstractGrainProjection(
        Id,
        Name,
        ProjectId,
        DisplayName,
        Description,
        Tags,
        CreatedUtc,
        UpdatedUtc);
