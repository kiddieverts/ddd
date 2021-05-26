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

        private List<RecordingAggregate> _unsavedAggregates { get; set; } = new List<RecordingAggregate>();

        private void ClearUnsavedAggregates()
        {
            _unsavedAggregates.ForEach(agg => agg.ClearUncommittedEvents());
            _unsavedAggregates.Clear();
        }

        public void AddToUnsaved(RecordingAggregate agg) => _unsavedAggregates.Add(agg);

        public async Task Commit()
        {
            // Randomly throw to emulate when db throws.
            if (DateTime.Now.Second % 2 == 0) throw new Exception("Error saving to db");

            await _db.Commit();
            ClearUnsavedAggregates();
        }
    }
}