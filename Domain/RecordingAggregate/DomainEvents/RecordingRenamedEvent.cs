using System;

namespace DDDExperiment
{
    public record RecordingRenamedEvent(Guid Id, string Name) : IDomainEvent;
}