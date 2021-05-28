using System;

namespace MyRental
{
    public class RecordingAggregate : AggregateRoot
    {
        public RecordingAggregate(long version) : base(version) { }

        public string Name { get; private set; }

        public record CreateFromDbInput(Guid Id, string Name);
        public static RecordingAggregate CreateFromDb(CreateFromDbInput r, long? version = -1)
        {
            if (r is null) throw new NullReferenceException(nameof(r));
            var agg = new RecordingAggregate((long)version);
            agg.Id = r.Id;
            agg.Name = r.Name;
            return agg;
        }

        public static RecordingAggregate Create(Guid id, string name, string artist, int year)
        {
            // TODO: Validation
            var agg = new RecordingAggregate(AggregateRoot.InitalVersion);
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

        private void Apply(RecordingCreatedEvent e)
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

        private void Apply(RecordingRenamedEvent e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        protected override void ApplyEvent(IDomainEvent ev)
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