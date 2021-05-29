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

            if (!result.IsSuccess)
            {
                // TODO: Global error handling.
                throw new Exception("Villa. " + result.Errors.Aggregate((agg, curr) => agg + "\n" + curr));
            }

            return "ok";
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

            public async Task<Result<Unit>> Handle(Command command)
            {
                // TODO: Auth
                // TODO: Validate input

                var aggResult = RecordingAggregate.Create(
                    new TrackId(command.Id),
                    TrackName.TryCreate(command.Name).Value,
                    ArtistName.TryCreate(command.Artist).Value,
                    new Year(command.Year));

                if (!aggResult.IsSuccess) return Result<Unit>.Failure(aggResult.Errors);

                var agg = aggResult.Value;

                await _recordingRepo.Save(agg);

                var isSuccess = false;
                try
                {
                    await _unitOfWork.Commit(); // TODO: Handle failure
                    isSuccess = true;
                }
                catch
                {
                    isSuccess = false;
                }

                Console.WriteLine(isSuccess ? "Saving success" : "Saving failed");

                return Result<Unit>.Succeed(new Unit());
            }
        }
    }
}