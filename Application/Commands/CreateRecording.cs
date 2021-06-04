using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{
    public class CreateRecordingController : ApiControllerBase
    {
        public CreateRecordingController(IMediator mediator, IUnitOfWork unitOfWork) : base(mediator, unitOfWork) { }

        [HttpPost]
        [Route("test")]
        public async Task<ActionResult<Unit>> CreateRecording([FromBody] CreateRecording.Command command)
        {
            var result = await _mediator.Command(command).SelectMany(Commit);
            return result.ToActionResult(this);
        }
    }

    public class CreateRecording
    {
        public record Command(Guid Id, string Name, string Artist, int Year) : ICommand;

        public class Handler : ICommandProcessHandler<Command>, ICommandHandler<Command>
        {
            private readonly IUserService _userService;
            private readonly RecordingRepository _recordingRepo;

            public Handler(IUserService userService, RecordingRepository recordingRepository)
            {
                _userService = userService;
                _recordingRepo = recordingRepository;
            }

            public async Task<Result<Unit>> Handle(Command command) =>
                await this.ProcessFullRequest(command);

            public async Task<Result<Unit>> DoMainWork(Command cmd) =>
                await ValidateInput(cmd)
                    .SelectMany(CreateAggregate)
                    .SelectMany(SaveAggregate);

            public Task<Result<Command>> Authorize(Command cmd) =>
                Task.FromResult(Result<Command>.Succeed(cmd));

            public Result<Command> ValidateInput(Command cmd) =>
                Validator.Create()
                    .Validate(cmd.Artist.IsEmpty(), ErrorType.ArtistNameEmpty)
                    .Validate(cmd.Name.IsEmpty(), ErrorType.NameNotAllowed)
                    .ToResult()
                    .Select(r => cmd);

            private Result<RecordingAggregate> CreateAggregate(Command cmd) =>
                ParallelValidator.Create(new ValidationObj())
                    .Validate(TrackName.TryCreate(cmd.Name), (o, r) => o with { TrackName = r })
                    .Validate(ArtistName.TryCreate(cmd.Artist), (o, r) => o with { ArtistName = r })
                    .ToResult()
                    .SelectMany(res => RecordingAggregate.Create(new TrackId(cmd.Id), res.TrackName, res.ArtistName, new Year(cmd.Year)));

            private async Task<Result<Unit>> SaveAggregate(RecordingAggregate agg) => await _recordingRepo.Save(agg);
            private record ValidationObj(TrackName TrackName = default(TrackName), ArtistName ArtistName = default(ArtistName));
        }
    }
}