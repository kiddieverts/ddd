using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyRental
{
    public abstract class RepositoryBase<T>
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly Database _db;
        private readonly IEventBus _eventBus;

        public RepositoryBase(IUnitOfWork unitOfWork, Database db, IEventBus eventBus)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _eventBus = eventBus;
        }

        public abstract T GetById(Guid id);

        public async Task Save(RecordingAggregate agg)
        {
            var events = agg.GetUncommittedEvents();
            await _unitOfWork.SaveEvents(events);
            agg.ClearUncommittedEvents();
        }
    }

    public class RecordingRepository : RepositoryBase<RecordingAggregate>, IRepository<RecordingAggregate>
    {
        public RecordingRepository(IUnitOfWork unitOfWork, Database db, IEventBus eventBus)
            : base(unitOfWork, db, eventBus) { }

        public override RecordingAggregate GetById(Guid id)
        {
            var recordingFromDb = _db.Recordings.FirstOrDefault(r => r.Id == id);
            return RecordingAggregate.CreateFromDb(recordingFromDb);
        }
    }
}