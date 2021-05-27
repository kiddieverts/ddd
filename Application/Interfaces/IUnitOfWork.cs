using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRental
{
    public interface IUnitOfWork
    {
        Task Save<T>(T agg) where T : AggregateRoot;
        Task Commit();
    }
}