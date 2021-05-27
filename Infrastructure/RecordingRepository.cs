using System;
using System.Linq;

namespace MyRental
{
    public class RecordingRepository : RepositoryBase<RecordingAggregate>, IRepository<RecordingAggregate>
    {
        public RecordingRepository(IUnitOfWork unitOfWork, Database db, IEventBus eventBus)
            : base(unitOfWork, db, eventBus) { }

        public override RecordingAggregate GetById(Guid id)
        {
            var recordingFromDb = _db.Recordings.FirstOrDefault(r => r.Id == id);
            long? version = GetAggregateVersion(id);

            return RecordingAggregate.CreateFromDb(recordingFromDb, version);
        }
    }
}