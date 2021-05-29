using System.Threading.Tasks;

namespace MyRental
{
    public class Mediator : IMediator
    {
        private readonly CreateRecording.Handler _createRecordingHandler;
        private readonly RenameRecording.Handler _renameRecordingHandler;

        public Mediator(CreateRecording.Handler createRecordingHandler, RenameRecording.Handler renameRecordingHandler)
        {
            _createRecordingHandler = createRecordingHandler;
            _renameRecordingHandler = renameRecordingHandler;
        }

        public async Task<Result<Unit>> Command(ICommand command)
        {
            Task<Result<Unit>> fn = command switch
            {
                CreateRecording.Command => _createRecordingHandler.Handle((CreateRecording.Command)command),
                RenameRecording.Command => _renameRecordingHandler.Handle((RenameRecording.Command)command),
                _ => throw new System.Exception("sadf"),
            };

            return await fn;
        }

        public Task<Result<T>> Query<T>(IQuery<T> query)
        {
            throw new System.NotImplementedException();
        }
    }
}