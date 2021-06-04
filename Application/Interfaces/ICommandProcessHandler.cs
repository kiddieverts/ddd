using System.Threading.Tasks;

namespace DDDExperiment
{
    public interface ICommandProcessHandler<TCommand>
    {
        Task<Result<TCommand>> Authorize(TCommand cmd);
        Task<Result<Unit>> DoMainWork(TCommand cmd);
        Result<TCommand> ValidateInput(TCommand cmd);
    }
}