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

        private void ClearUnsavedAggregates()
        {
            _uncommitedEvents.Clear();
        }

        public async Task SaveEvents(IEnumerable<IDomainEvent> events)
        {
            foreach (var ev in events)
            {
                _uncommitedEvents.Add(ev);
                await _eventBus.Publish(ev);
            }
        }

        public async Task Commit()
        {
            // Randomly throw to emulate when db throws.
            // if (DateTime.Now.Second % 2 == 0) throw new Exception("Error saving to db");

            await _db.Commit();
            ClearUnsavedAggregates();
        }
    }
}