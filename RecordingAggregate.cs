using System;

namespace MyRental
{
    public record RecordingAggregateCreatedEvent : IDomainEvent
    {
        public Guid Id;
        public string Name;
        public string Artist;
        public int Year;
    }

    public class RecordingAggregate : AggregateRoot
    {
        private RecordingAggregate() { }
        public string Name { get; private set; }

        public static RecordingAggregate FromDb(Recording r)
        {
            var agg = new RecordingAggregate();
            // RecordingAggregate.Create();
            agg.Name = r.Name;
            return agg;
        }

        public static RecordingAggregate Create(Guid id, string name, string artist, int year)
        {
            var agg = new RecordingAggregate();
            agg.RaiseEvent(new RecordingAggregateCreatedEvent
            {
                Id = id,
                Name = name,
                Artist = artist,
                Year = year
            });
            return agg;
        }

        public void Apply(RecordingAggregateCreatedEvent ev)
        {
            Id = ev.Id;
            Name = ev.Name;
        }
    }
}