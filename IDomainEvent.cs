using System;

namespace SFH.Repertoire.Domain
{
    public interface IDomainEvent
    {
        Guid _EventId { get; init; }
        int _EventVersion { get; init; }
        long _AggregateVersion { get; init; }
        string GetEventName();
    }
}