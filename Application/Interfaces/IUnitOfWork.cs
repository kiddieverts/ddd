using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRental
{
    public interface IUnitOfWork
    {
        Task SaveEvents(IEnumerable<IDomainEvent> events);
        Task Commit();
    }
}