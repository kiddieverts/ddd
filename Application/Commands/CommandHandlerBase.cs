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
                .SelectMany(ValidateInput)
                .SelectMany(DoMainWork);

        protected abstract Task<Result<Unit>> DoMainWork(TCommand cmd);
    }
}