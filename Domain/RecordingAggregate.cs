using System;

namespace MyRental
{
    public class RecordingAggregate : AggregateRoot
    {
        private RecordingAggregate() { }
        public string Name { get; private set; }

        public static RecordingAggregate Create(Guid id, string name, string artist, int year)
        {
            // TODO: Validation
            var agg = new RecordingAggregate();
            agg.RaiseEvent(new RecordingCreatedEvent
            {
                Id = id,
                Name = name,
                Artist = artist,
                Year = year
            });
            return agg;
        }

        public static RecordingAggregate CreateFromDb(Recording r)
        {
            var agg = new RecordingAggregate();
            agg.Name = r.Name;
            return agg;
        }

        public void Apply(RecordingCreatedEvent ev)
        {
            Id = ev.Id;
            Name = ev.Name;
        }

        protected override void ApplyEvent(IDomainEvent @event)
        {
            Action fn = @event switch
            {
                RecordingCreatedEvent => () => Apply((RecordingCreatedEvent)@event),
                _ => () => { }
            };

            fn();
        }
    }
}