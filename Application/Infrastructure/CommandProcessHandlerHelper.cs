using System.Threading.Tasks;

namespace DDDExperiment
{
    public static class CommandProcessHandlerHelper
    {
        public static async Task<Result<Unit>> ProcessFullRequest<TCommand>(this ICommandProcessHandler<TCommand> handler, TCommand command) =>
            await handler.Authorize(command)
                .SelectMany(handler.ValidateInput)
                .SelectMany(handler.DoMainWork);
    }
}