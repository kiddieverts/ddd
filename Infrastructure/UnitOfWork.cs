using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRental
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly IEventBus _eventBus;
        public readonly Database _db;

        public UnitOfWork(IEventBus eventBus, Database db)
        {
            _db = db;
            _eventBus = eventBus;
        }

        private List<IDomainEvent> _uncommitedEvents { get; set; } = new List<IDomainEvent> { };
        private List<PersistedEvent> _uncommitedPersistedEvents { get; set; } = new List<PersistedEvent> { };

        private void Cleanup()
        {
            _uncommitedEvents.Clear();
            _uncommitedPersistedEvents.Clear();
        }

        public async Task Save<T>(T agg) where T : AggregateRoot
        {
            var i = agg.Version;
            foreach (var ev in agg.GetUncommittedEvents())
            {
                i++; // TODO: <-- hmmmm
                var persistedEvent = new PersistedEvent(ev.ToString(), i, agg.Id);

                // TODO: Hmmm.... make dry
                _uncommitedEvents.Add(ev);
                _uncommitedPersistedEvents.Add(persistedEvent);

                await _eventBus.Publish(ev);
            }
        }

        public async Task Commit()
        {
            foreach (var persistedEvent in _uncommitedPersistedEvents)
            {
                await _db.AddAction(() => _db.Events.Add(persistedEvent));
                Console.WriteLine(persistedEvent);
            }

            await _db.Commit();
            Cleanup();
        }
    }
}