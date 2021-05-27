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
            var ev = new RecordingCreatedEvent
            {
                Id = id,
                Name = name,
                Artist = artist,
                Year = year
            };

            agg.RaiseEvent(ev, e =>
            {
                agg.Id = e.Id;
                agg.Name = e.Name;
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
            var ev = new RecordingRenamedEvent(Id, name);
            RaiseEvent(ev, e =>
            {
                Id = e.Id;
                Name = e.Name;
            });
        }
    }
}