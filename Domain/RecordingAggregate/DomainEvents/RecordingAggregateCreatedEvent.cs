using System;

namespace MyRental
{
    public record RecordingCreatedEvent : IDomainEvent
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Artist { get; init; }
        public int Year { get; init; }
    }
}