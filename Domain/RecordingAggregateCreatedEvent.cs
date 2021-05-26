using System;

namespace MyRental
{
    public record RecordingCreatedEvent : IDomainEvent
    {
        public Guid Id;
        public string Name;
        public string Artist;
        public int Year;
    }
}