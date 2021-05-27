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
            await _unitOfWork.Save(agg);
            agg.ClearUncommittedEvents();
        }

        protected long? GetAggregateVersion(Guid aggregateId) =>
            _db.Events
                .Where(e => e.AggregateId == aggregateId)
                .Max(x => x is not null ? x.AggregateVersion : null);
    }
}