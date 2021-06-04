using System.Threading.Tasks;

namespace MyRental
{
    public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public CommandHandlerBase(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        protected abstract Task<Result<TCommand>> Authorize(TCommand cmd);

        protected abstract Result<TCommand> ValidateInput(TCommand cmd);

        public async Task<Result<Unit>> Handle(TCommand command) =>
            await Authorize(command)
                .SelectMany(r => Task.FromResult(ValidateInput(r))) // TODO: Lift...
                .SelectMany(DoMainWork)
                .SelectMany(Commit);
        protected async Task<Result<Unit>> Commit(Unit u) => await _unitOfWork.Commit();

        protected abstract Task<Result<Unit>> DoMainWork(TCommand cmd);
    }
}