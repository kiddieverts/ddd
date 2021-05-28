using System;
using System.Linq;

namespace MyRental
{
    public class RecordingRepository : RepositoryBase<RecordingAggregate>, IRepository<RecordingAggregate>
    {
        public RecordingRepository(IUnitOfWork unitOfWork, Database db)
            : base(unitOfWork, db) { }

        public override RecordingAggregate GetById(Guid id)
        {
            var recordingFromDb = _db.Recordings.FirstOrDefault(r => r.Id == id);
            long? version = GetAggregateVersion(id);
            var input = new RecordingAggregate.CreateFromDbInput(recordingFromDb.Id, recordingFromDb.Name);

            return RecordingAggregate.CreateFromDb(input, version);
        }
    }
}