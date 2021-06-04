using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{
    public class CreateRecordingController : ApiControllerBase
    {
        public CreateRecordingController(IMediator mediator, IUnitOfWork unitOfWork) : base(mediator, unitOfWork) { }

        [HttpPost]
        [Route("test")]
        public async Task<ActionResult<string>> CreateRecording([FromBody] CreateRecording.Command command)
        {
            var result = await _mediator.Command(command).SelectMany(Commit);

            return result.Map(r => Ok("ok"), e => ValidationProblem(e.First().Errors.Aggregate("", (agg, curr) => agg + curr.ToString() + " ... "))); // TODO: Make generic
        }
    }

    public class CreateRecording
    {
        public record Command(Guid Id, string Name, string Artist, int Year) : ICommand;

        public class Handler : CommandHandlerBase<Command>
        {
            private readonly RecordingRepository _recordingRepo;
            public Handler(IUnitOfWork unitOfWork, IUserService userService, RecordingRepository recordingRepository)
                : base(unitOfWork, userService)
            {
                _recordingRepo = recordingRepository;
            }

            protected async override Task<Result<Unit>> DoMainWork(Command cmd) =>
                await ValidateInput(cmd)
                    .SelectMany(CreateAggregate)
                    .SelectMany(SaveAggregate);

            protected override Task<Result<Command>> Authorize(Command cmd) =>
                Task.FromResult(Result<Command>.Succeed(cmd));

            protected override Result<Command> ValidateInput(Command cmd) =>
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

            private record ValidationObj(TrackName TrackName = default(TrackName), ArtistName ArtistName = default(ArtistName));
            private async Task<Result<Unit>> SaveAggregate(RecordingAggregate agg) => await _recordingRepo.Save(agg);
        }
    }
}