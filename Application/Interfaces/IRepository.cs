using System;
using System.Threading.Tasks;

namespace MyRental
{
    public interface IRepository<T>
    {
        public T GetById(Guid id);
        public Task Save(T agg);
    }
}