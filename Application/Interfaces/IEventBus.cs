using System.Threading.Tasks;

namespace DDDExperiment
{
    public interface IEventBus
    {
        public Task Publish(IDomainEvent ev);
    }
}