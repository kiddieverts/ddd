using System.Linq;
using System.Threading.Tasks;

namespace MyRental
{
    public class RecordingRenamedEventHandler
    {
        private readonly Database _db;
        public RecordingRenamedEventHandler(Database db)
        {
            _db = db;
        }

        public async Task Handle(RecordingRenamedEvent ev)
        {
            var recording = _db.Recordings.FirstOrDefault(r => r.Id == ev.Id);

            // TODO: Use real database instead
            await _db.AddAction(() =>
            {
                var newRecording = recording with { Name = ev.Name };
                var idx = _db.Recordings.FindIndex(r => r.Id == ev.Id);
                _db.Recordings[idx] = newRecording;
            });
        }
    }
}