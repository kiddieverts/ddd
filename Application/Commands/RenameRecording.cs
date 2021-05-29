using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{
    public class RenameRecordingController : BaseController
    {
        public RenameRecordingController(IMediator mediator) : base(mediator) { }

        [HttpGet("test/rename")]
        public async Task<ActionResult<string>> RenameRecording([FromQuery] Guid id)
        {
            var command = new RenameRecording.Command(id, "Test");

            await _mediator.Command(command);

            return "ok";
        }
    }

    public class RenameRecording
    {
        public record Command(Guid Id, string Name) : ICommand;

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
                // TODO: Validation
                var agg = _recordingRepo.GetById(command.Id);
                agg.Rename(command.Name);
                await _recordingRepo.Save(agg);
                await _unitOfWork.Commit();

                var agg2 = _recordingRepo.GetById(command.Id);
                Console.WriteLine("FROM DB ... " + agg2.Name);

                return Result<Unit>.Succeed(new Unit());
            }
        }
    }
}