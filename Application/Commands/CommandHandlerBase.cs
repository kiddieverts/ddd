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

        protected abstract Result<TCommand> Authorize(TCommand cmd);

        protected abstract Result<TCommand> TryValidateInput(TCommand cmd);

        public async Task<Result<Unit>> Handle(TCommand command) =>
            await Authorize(command)
                .SelectMany(TryValidateInput)
                .SelectMany(DoMainWork)
                .SelectMany(TryCommit);
        protected async Task<Result<Unit>> TryCommit(Unit u) => await _unitOfWork.Commit();

        protected abstract Result<Unit> DoMainWork(TCommand cmd);
    }
}