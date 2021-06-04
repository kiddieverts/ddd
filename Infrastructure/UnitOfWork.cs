using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRental
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly IEventBus _eventBus;
        public readonly Database _db;
        private bool _commitHasFailed = false;

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

        public Task Save<T>(T agg) where T : AggregateRoot
        {
            var x = agg.GetUncommittedEvents()
                .Select((ev, i) =>
                {
                    var version = agg.Version + i + 1;
                    var persistedEvent = new PersistedEvent(ev.ToString(), version, agg.Id); // TODO: Store as Serialized JSON
                    return (ev, persistedEvent);
                });

            var (domainEvents, persistedEvents) = (x.Select(e => e.ev), x.Select(e => e.persistedEvent));

            _uncommitedEvents = _uncommitedEvents.Concat(domainEvents).ToList();
            _uncommitedPersistedEvents = _uncommitedPersistedEvents.Concat(persistedEvents).ToList();

            return Task.CompletedTask;
        }

        public async Task<Result<Unit>> Commit()
        {
            if (_commitHasFailed == true) return Result<Unit>.Failure(SystemError.Create(ErrorType.CommitHasFailed));

            try
            {
                foreach (var persistedEvent in _uncommitedPersistedEvents)
                {
                    await _db.AddAction(() => _db.Events.Add(persistedEvent));
                    Console.WriteLine(persistedEvent);
                }

                foreach (var ev in _uncommitedEvents)
                {
                    await _eventBus.Publish(ev);
                }

                await _db.Commit();
                Cleanup();
                return Result<Unit>.Succeed(new Unit());
            }
            catch (Exception e)
            {
                _commitHasFailed = true;
                return Result<Unit>.Failure(SystemError.Create(e));
            }
        }
    }
}