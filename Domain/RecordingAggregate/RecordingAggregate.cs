using System;

namespace DDDExperiment
{
    public class RecordingAggregate : AggregateRoot
    {
        public string Name { get; private set; }

        public RecordingAggregate(long version) : base(version) { }

        public static Result<RecordingAggregate> Create(TrackId id, TrackName name, ArtistName artist, Year year)
        {
            if (!IsTrackNameValid(name))
                return Result<RecordingAggregate>.Failure(ValidationError.Create(ErrorType.NameNotAllowed));

            var agg = new RecordingAggregate(AggregateRoot.InitalVersion);
            var ev = new RecordingCreatedEvent
            {
                Id = id.Value,
                Name = name.Value,
                Artist = artist.Value,
                Year = year.Value
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

        public Result<RecordingAggregate> Rename(TrackName name)
        {
            if (!IsTrackNameValid(name))
                return Result<RecordingAggregate>.Failure(ValidationError.Create(ErrorType.NameNotAllowed));

            var ev = new RecordingRenamedEvent(Id, name.ToString());
            RaiseEvent(ev);

            return Result<RecordingAggregate>.Succeed(this);
        }

        private void Apply(RecordingRenamedEvent e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        private static bool IsTrackNameValid(TrackName name) => name.Value != "Random";

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