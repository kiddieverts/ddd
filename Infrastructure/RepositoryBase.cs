using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyRental
{
    public abstract class RepositoryBase<T>
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly Database _db;

        public RepositoryBase(IUnitOfWork unitOfWork, Database db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }

        // public abstract T GetById(Guid id);

        public async Task<Result<Unit>> Save(RecordingAggregate agg)
        {
            try
            {
                await _unitOfWork.Save(agg);
                agg.ClearUncommittedEvents();
                return Result<Unit>.Succeed(new Unit());
            }
            catch (Exception e)
            {
                return Result<Unit>.Failure(SystemError.Create(e));
            }
        }

        protected long? GetAggregateVersion(Guid aggregateId) =>
            _db.Events
                .Where(e => e.AggregateId == aggregateId)
                .Max(x => x is not null ? x.AggregateVersion : null);
    }
}