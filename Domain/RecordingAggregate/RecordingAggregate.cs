using System;

namespace MyRental
{
    public class RecordingAggregate : AggregateRoot
    {
        public string Name { get; private set; }

        public RecordingAggregate(long version) : base(version) { }

        public static Result<RecordingAggregate> Create(Guid id, string name, string artist, int year)
        {
            if (name == "") return Result<RecordingAggregate>.Failure("Error creating RecordingAggregate");

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
            return Result<RecordingAggregate>.Succeed(agg);
        }

        private void Apply(RecordingCreatedEvent e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        public record CreateFromDbInput(Guid Id, string Name);
        public static RecordingAggregate CreateFromDb(CreateFromDbInput r, long? version = -1)
        {
            if (r is null) throw new NullReferenceException(nameof(r));
            var agg = new RecordingAggregate((long)version);
            agg.Id = r.Id;
            agg.Name = r.Name;
            return agg;
        }

        public Result<Unit> Rename(string name)
        {
            // TODO: Validation
            if (name == "") return Result<Unit>.Failure("Error renaming...");

            var ev = new RecordingRenamedEvent(Id, name);
            RaiseEvent(ev);

            return Result<Unit>.Succeed(new Unit());
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
                _ => () => throw new Exception("Not supported") // TODO: Hmmm
            };

            fn();
        }
    }
}