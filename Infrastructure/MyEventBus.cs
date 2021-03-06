using System;
using System.Threading.Tasks;

namespace DDDExperiment
{
    public class MyEventBus : IEventBus
    {
        private readonly Database _db;

        public MyEventBus(Database db)
        {
            _db = db;
        }

        public async Task Publish(IDomainEvent ev)
        {
            Task handle = ev switch
            {
                RecordingCreatedEvent => new RecordingCreatedEventHandler(_db).Handle((RecordingCreatedEvent)ev),
                RecordingRenamedEvent => new RecordingRenamedEventHandler(_db).Handle((RecordingRenamedEvent)ev),
                _ => throw new Exception("")
            };

            await handle;
        }
    }
}