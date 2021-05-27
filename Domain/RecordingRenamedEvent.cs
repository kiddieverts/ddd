using System;

namespace MyRental
{
    public record RecordingRenamedEvent(Guid Id, string Name) : IDomainEvent;
}