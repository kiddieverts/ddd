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
            agg.Id = r.Id;
            return agg;
        }

        public void Rename(string name)
        {
            // TODO: Validation
            RaiseEvent(new RecordingRenamedEvent(Id, name));
        }

        public void Apply(RecordingCreatedEvent ev)
        {
            Id = ev.Id;
            Name = ev.Name;
        }

        public void Apply(RecordingRenamedEvent ev)
        {
            Name = ev.Name;
        }

        protected override void ApplyEvent(IDomainEvent @event)
        {
            Action fn = @event switch
            {
                RecordingCreatedEvent => () => Apply((RecordingCreatedEvent)@event),
                RecordingRenamedEvent => () => Apply((RecordingRenamedEvent)@event),
                _ => () => { }
            };

            fn();
        }
    }
}