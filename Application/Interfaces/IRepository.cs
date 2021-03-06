using System;
using System.Threading.Tasks;

namespace DDDExperiment
{
    public interface IRepository<T>
    {
        public Task<Result<T>> GetById(Guid id);
        public Task<Result<Unit>> Save(T agg);
    }
}