using System;
using System.Linq;
using System.Threading.Tasks;

namespace DDDExperiment
{
    public class RecordingRepository : RepositoryBase<RecordingAggregate>, IRepository<RecordingAggregate>
    {
        public RecordingRepository(IUnitOfWork unitOfWork, Database db)
            : base(unitOfWork, db) { }

        public Task<Result<RecordingAggregate>> GetById(Guid id)
        {
            var recordingFromDb = _db.Recordings.FirstOrDefault(r => r.Id == id);

            if (recordingFromDb is null)
                return Task.FromResult(Result<RecordingAggregate>.Failure(new NotFoundError()));

            long? version = GetAggregateVersion(id);
            var input = new RecordingAggregate.CreateFromDbInput(recordingFromDb.Id, recordingFromDb.Name);

            return Task.FromResult(Result<RecordingAggregate>.Succeed(RecordingAggregate.CreateFromDb(input, version)));
        }
    }
}