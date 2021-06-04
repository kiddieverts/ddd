using System;
using System.Threading.Tasks;

namespace DDDExperiment
{
    public class RenameRecording
    {
        public record Command(Guid Id, string Name) : ICommand;

        public class Handler : ICommandHandler<Command>, ICommandProcessHandler<Command>
        {
            private readonly RecordingRepository _recordingRepo;

            public Handler(RecordingRepository recordingRepository)
            {
                _recordingRepo = recordingRepository;
            }

            public async Task<Result<Unit>> Handle(Command command) => await this.ProcessFullRequest(command);

            public async Task<Result<Unit>> DoMainWork(Command command)
            {
                var getAggregateResult = _recordingRepo.GetById(command.Id);

                Func<RecordingAggregate, Task<Result<Unit>>> saveAggregate = agg =>
                    _recordingRepo.Save(agg);

                Func<RecordingAggregate, Result<RecordingAggregate>> rename = agg =>
                    TrackName.TryCreate(command.Name).SelectMany(name => agg.Rename(name));

                return await getAggregateResult
                    .SelectMany(rename)
                    .SelectMany(saveAggregate);
            }

            public Task<Result<Command>> Authorize(Command cmd) => Task.FromResult(Result<Command>.Succeed(cmd));

            public Result<Command> ValidateInput(Command cmd) => Result<Command>.Succeed(cmd);
        }
    }
}