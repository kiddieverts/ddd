using System.Threading.Tasks;

namespace MyRental
{
    public interface IUnitOfWork
    {
        void AddToUnsaved(RecordingAggregate agg);
        Task Commit();
    }
}