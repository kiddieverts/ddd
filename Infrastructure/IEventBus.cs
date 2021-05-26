using System.Threading.Tasks;

namespace MyRental
{
    public interface IEventBus
    {
        public Task Publish(IDomainEvent ev);
    }
}