using System;
using System.Threading.Tasks;

namespace MyRental
{
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