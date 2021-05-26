using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRental
{
    public class RecordingAggregateCreatedEventHandler
    {
        private readonly Database _db;
        public RecordingAggregateCreatedEventHandler(Database db)
        {
            _db = db;
        }

        public Task Handle(RecordingAggregateCreatedEvent ev)
        {
            _db.AddAction(() =>
            {
                _db.Recordings.Add(new Recording(ev.Id, ev.Name, ev.Artist, ev.Year));
            });
            return Task.CompletedTask;
        }
    }

    public interface IEventBus
    {
        public Task Publish(IDomainEvent ev);
    }

    public class MyEventBus : IEventBus
    {
        private readonly Database _db;

        public MyEventBus(Database db)
        {
            _db = db;
        }

        public async Task Publish(IDomainEvent ev)
        {
            Task handle = ev switch
            {
                RecordingAggregateCreatedEvent => new RecordingAggregateCreatedEventHandler(_db).Handle((RecordingAggregateCreatedEvent)ev),
                _ => throw new Exception("")
            };

            await handle;
        }
    }

    public class UnitOfWork
    {
        public readonly IEventBus _eventBus;
        public readonly Database _db;
        public RecordingRepository RecordingRepo;

        public UnitOfWork(IEventBus eventBus, Database db, RecordingRepository recordingRepo)
        {
            _eventBus = eventBus;
            _db = db;
            RecordingRepo = recordingRepo;
        }

        private List<RecordingAggregate> _unsavedAggregates { get; set; } = new List<RecordingAggregate>();

        private void ClearUnsaved()
        {
            _unsavedAggregates.ForEach(agg => agg.ClearUncommittedEvents());
            _unsavedAggregates.Clear();
        }
        private void AddToUnsaved(RecordingAggregate agg) => _unsavedAggregates.Add(agg);

        public async Task Save(RecordingAggregate agg)
        {
            var events = agg.GetUncommittedEvents();

            foreach (var ev in events)
            {
                await _eventBus.Publish(ev);
            }

            AddToUnsaved(agg);
        }

        public RecordingAggregate GetRecordingAggregate(Guid id)
        {
            return RecordingAggregate.Create(Guid.NewGuid(), "asdf", "adsf", 1);
        }

        public async Task Commit()
        {
            await CommitTransactionToDatabase();
            ClearUnsaved();
        }

        private Task CommitTransactionToDatabase()
        {
            var s = DateTime.Now.Second;
            if (s % 2 == 0) throw new Exception("Error saving to db");
            _db.Actions.ForEach(a => a());

            return Task.CompletedTask;
        }
    }

    public interface IRepository<T>
    {
        public T GetById(Guid id);
    }

    public class RecordingRepository : IRepository<RecordingAggregate>
    {
        private readonly Database _db;

        public RecordingRepository(Database database)
        {
            _db = database;
        }

        public RecordingAggregate GetById(Guid id)
        {
            var r = _db.Recordings.FirstOrDefault(r => r.Id == id);

            return RecordingAggregate.FromDb(r);
        }

    }
}