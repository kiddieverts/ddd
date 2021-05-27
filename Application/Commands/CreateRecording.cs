using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyRental
{
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

            public async Task Handle(Command command)
            {
                var agg = RecordingAggregate.Create(command.Id, command.Name, command.Artist, command.Year);
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
            }
        }
    }
}