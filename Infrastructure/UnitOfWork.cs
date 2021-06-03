using System;
using System.Collections.Generic;
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
            var i = agg.Version;
            foreach (var ev in agg.GetUncommittedEvents())
            {
                i++; // TODO: <-- hmmmm
                var persistedEvent = new PersistedEvent(ev.ToString(), i, agg.Id); // TODO: Store as Serialized JSON

                _uncommitedEvents.Add(ev);
                _uncommitedPersistedEvents.Add(persistedEvent);
            }

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