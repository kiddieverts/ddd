using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDExperiment
{
    public interface IUnitOfWork
    {
        Task Save<T>(T agg) where T : AggregateRoot;
        Task<Result<Unit>> Commit();
    }
}