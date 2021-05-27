using System;

namespace MyRental
{
    public class RecordingAggregate : AggregateRoot
    {
        private RecordingAggregate() { }
        public string Name { get; private set; }

        public static RecordingAggregate CreateFromDb(Recording r)
        {
            var agg = new RecordingAggregate();
            agg.Name = r.Name;
            agg.Id = r.Id;
            return agg;
        }

        public static RecordingAggregate Create(Guid id, string name, string artist, int year)
        {
            // TODO: Validation
            var agg = new RecordingAggregate();
            var ev = new RecordingCreatedEvent
            {
                Id = id,
                Name = name,
                Artist = artist,
                Year = year
            };

            agg.RaiseEvent(ev);
            return agg;
        }

        public void Apply(RecordingCreatedEvent e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        public void Rename(string name)
        {
            // TODO: Validation
            var ev = new RecordingRenamedEvent(Id, name);
            RaiseEvent(ev);
        }

        public void Apply(RecordingRenamedEvent e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        public override void ApplyEvent(IDomainEvent ev)
        {
            Action fn = ev switch
            {
                RecordingCreatedEvent => () => Apply((RecordingCreatedEvent)ev),
                RecordingRenamedEvent => () => Apply((RecordingRenamedEvent)ev),
                _ => () => { }
            };

            fn();
        }
    }
}