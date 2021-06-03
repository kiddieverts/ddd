using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{
    public class CreateRecordingController : BaseController
    {
        public CreateRecordingController(IMediator mediator) : base(mediator) { }

        [HttpPost]
        [Route("test")]
        public async Task<ActionResult<string>> CreateRecording([FromBody] CreateRecording.Command command)
        {
            var result = await _mediator.Command(command);
            return result.Map(r => Ok("ok"), e => ValidationProblem(e.Errors.Aggregate("", (agg, curr) => agg + curr.ToString() + " ... "))); // TODO: Make generic
        }
    }

    public class CreateRecording
    {
        public record Command(Guid Id, string Name, string Artist, int Year) : ICommand;

        public class Handler : ICommandHandler<Command>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly RecordingRepository _recordingRepo;
            private readonly IUserService _userService;

            public Handler(IUnitOfWork unitOfWork, RecordingRepository recordingRepository, IUserService userService)
            {
                _unitOfWork = unitOfWork;
                _recordingRepo = recordingRepository;
                _userService = userService;
            }

            public async Task<Result<Unit>> Handle(Command command) =>
                await Authorize(_userService.GetUserId(), command)
                    .SelectMany(TryValidateInput)
                    .SelectMany(TryCreateAggregate)
                    .SelectMany(TrySaveAggregate)
                    .SelectMany(TryCommit);

            private static Result<Command> Authorize(Guid uid, Command cmd) => true
                ? Result<Command>.Succeed(cmd)
                : Result<Command>.Failure(NotAuthorizedError.Create(ErrorType.AuthorizationFailed));

            private static Result<Command> TryValidateInput(Command cmd) =>
                ErrorHelper.CreateEmptyErrorList()
                    .Validate(cmd.Artist.IsEmpty(), ErrorType.ArtistNameEmpty)
                    .Validate(cmd.Name.IsEmpty(), ErrorType.NameNotAllowed)
                    .CheckForErrors()
                    .Select(r => cmd);

            private Result<RecordingAggregate> TryCreateAggregate(Command cmd)
            {
                var id = new TrackId(cmd.Id);
                var name = TrackName.TryCreate(cmd.Name);
                var artist = ArtistName.TryCreate(cmd.Artist);
                var year = new Year(cmd.Year);

                var isSuccess = ErrorHelper.CreateEmptyErrorList()
                    .Validate(!name.IsSuccess(), ErrorType.ArtistNameEmpty)
                    .Validate(!artist.IsSuccess(), ErrorType.NameNotAllowed)
                    .CheckForErrors()
                    .IsSuccess();

                return isSuccess
                    ? RecordingAggregate.Create(id, name.GetValue(), artist.GetValue(), year)
                    : Result<RecordingAggregate>.Failure(ValidationError.Create(ErrorType.InvalidRecordingAggregate));
            }

            private async Task<Result<Unit>> TrySaveAggregate(RecordingAggregate agg) => await _recordingRepo.Save(agg);
            private async Task<Result<Unit>> TryCommit(Unit u) => await _unitOfWork.Commit();
        }
    }
}