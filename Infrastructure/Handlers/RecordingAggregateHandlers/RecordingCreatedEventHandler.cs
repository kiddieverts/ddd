using System.Threading.Tasks;

namespace MyRental
{
    public class RecordingCreatedEventHandler
    {
        private readonly Database _db;
        public RecordingCreatedEventHandler(Database db)
        {
            _db = db;
        }

        public async Task Handle(RecordingCreatedEvent ev)
        {
            // TODO: Use real database instead
            await _db.AddAction(() => _db.Recordings.Add(new Recording(ev.Id, ev.Name, ev.Artist, ev.Year)));
        }
    }
}