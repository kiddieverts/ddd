using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{

    [ApiController]
    [Route("test")]
    public class RenameRecordingController : ControllerBase
    {
        private readonly CreateRecording.Handler _createRecordinghandler;
        private readonly RenameRecording.Handler _renameRecordinghandler;

        public RenameRecordingController(CreateRecording.Handler handler, RenameRecording.Handler renameRecordinghandler)
        {
            _createRecordinghandler = handler;
            _renameRecordinghandler = renameRecordinghandler;
        }

        [HttpGet("rename")]
        public async Task<ActionResult<string>> RenameRecording([FromQuery] Guid id)
        {
            var command = new RenameRecording.Command(id, "Test");

            await _renameRecordinghandler.Handle(command);

            return "ok";
        }
    }

    public class RenameRecording
    {
        public record Command(Guid Id, string Name);

        public class Handler
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly RecordingRepository _recordingRepo;

            public Handler(IUnitOfWork unitOfWork, RecordingRepository recordingRepository)
            {
                _unitOfWork = unitOfWork;
                _recordingRepo = recordingRepository;
            }

            public async Task Handle(Command command)
            {
                // TODO: Validation
                var agg = _recordingRepo.GetById(command.Id);
                agg.Rename(command.Name);
                await _recordingRepo.Save(agg);
                await _unitOfWork.Commit();

                var agg2 = _recordingRepo.GetById(command.Id);
                Console.WriteLine("FROM DB ... " + agg2.Name);
            }
        }
    }
}