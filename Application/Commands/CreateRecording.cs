using System;
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
            return result.Map(r => Ok("ok"), e => ValidationProblem(e.Msg)); // TODO: Make generic
        }
    }

    public class CreateRecording
    {
        public record Command(Guid Id, string Name, string Artist, int Year) : ICommand;

        public class Handler : ICommandHandler<Command>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly RecordingRepository _recordingRepo;

            public Handler(IUnitOfWork unitOfWork, RecordingRepository recordingRepository)
            {
                _unitOfWork = unitOfWork;
                _recordingRepo = recordingRepository;
            }

            public Task<Result<Unit>> Handle(Command command)
            {
                var userId = Guid.Parse("68c6ef9d-8e25-48a0-b577-5f8ff1fae643"); // TODO: Where does this come from??

                Func<Guid, Command, Result<Command>> authorize = static (Guid uid, Command cmd) => uid == Guid.Parse("68c6ef9d-8e25-48a0-b577-5f8ff1fae643")
                    ? Result<Command>.Succeed(cmd)
                    : Result<Command>.Failure("Auth error: Not correct user id"); // TODO: Dont use error strings

                Func<Command, Result<Command>> validateInput = static (Command cmd) => // TODO: Refactor !!!
                {
                    var conditions = new Condition[]
                    {
                        new Condition(cmd.Id.IsEmpty(), "Id can not be empty guid"),
                        new Condition(cmd.Name.IsEmpty(), "Name can not be empty"),
                        new Condition(cmd.Artist.IsEmpty(), "Artist can not be empty"),
                        new Condition(cmd.Year < 1900, "Year has to be after 1900")
                    };

                    return conditions.CheckForErrors().Select(r => cmd);
                };

                Func<Command, Result<RecordingAggregate>> tryCreateAggregate = static (Command cmd) =>
                {
                    var id = new TrackId(cmd.Id);
                    var name = TrackName.TryCreate(cmd.Name);
                    var artist = ArtistName.TryCreate(cmd.Artist);
                    var year = new Year(cmd.Year);

                    var c = new Condition[]
                    {
                        new Condition(!name.IsSuccess(), "Name is not correct"),
                        new Condition(!artist.IsSuccess(), "Artist is not correct")
                    };

                    var validation = c.CheckForErrors();

                    return validation.IsSuccess()
                        ? RecordingAggregate.Create(id, name.GetValue(), artist.GetValue(), year)
                        : Result<RecordingAggregate>.Failure("Failed to create aggregate");
                };

                Func<RecordingAggregate, Task<Result<Unit>>> trySaveAggregate = async agg =>
                    await _recordingRepo.Save(agg);

                Func<Unit, Task<Result<Unit>>> tryCommit = async agg =>
                    await _unitOfWork.Commit();

                Func<Guid, Command, Task<Result<Unit>>> createRecording = async (userId, command) =>
                    await authorize(userId, command)
                        .SelectMany(validateInput)
                        .SelectMany(tryCreateAggregate)
                        .SelectMany(trySaveAggregate)
                        .SelectMany(tryCommit);

                return createRecording(userId, command);
            }
        }
    }
}