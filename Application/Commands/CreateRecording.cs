using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{
    [ApiController]
    [Route("test")]
    public class CreateRecordingController : ControllerBase
    {
        private readonly CreateRecording.Handler _createRecordinghandler;

        public CreateRecordingController(CreateRecording.Handler handler)
        {
            _createRecordinghandler = handler;
        }

        [HttpGet]
        public async Task<ActionResult<string>> CreateRecording()
        {
            var command = new CreateRecording.Command(Guid.NewGuid(), "Ok Computer", "Radiohead", 1997);

            var result = await _createRecordinghandler.Handle(command);

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
        public record Command(Guid Id, string Name, string Artist, int Year);

        public class Handler
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly RecordingRepository _recordingRepo;

            public Handler(IUnitOfWork unitOfWork, RecordingRepository recordingRepository)
            {
                _unitOfWork = unitOfWork;
                _recordingRepo = recordingRepository;
            }

            public async Task<Result<Guid>> Handle(Command command)
            {
                var aggResult = RecordingAggregate.Create(command.Id, command.Name, command.Artist, command.Year);

                if (!aggResult.IsSuccess) return Result<Guid>.Failure(aggResult.Errors);

                var agg = aggResult.Value;

                await _recordingRepo.Save(agg);

                var isSuccess = false;
                try
                {
                    await _unitOfWork.Commit();
                    isSuccess = true;
                }
                catch
                {
                    isSuccess = false;
                }

                Console.WriteLine(isSuccess ? "Saving success" : "Saving failed");

                if (isSuccess)
                {
                    var agg2 = _recordingRepo.GetById(command.Id);
                    Console.WriteLine("FROM DB ... " + agg2.Name + ".. Id .. " + agg.Id);
                }
                Console.WriteLine("Events uncommitted ... " + agg.GetUncommittedEvents().Count());

                return Result<Guid>.Succeed(command.Id);
            }
        }
    }
}