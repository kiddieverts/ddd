using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRental
{
    public record PersistedEvent(string ev, long AggregateVersion, Guid AggregateId);
    public record Recording(Guid Id, string Name, string Artist, int Year);

    public class Database
    {
        public List<Recording> Recordings = new List<Recording>();
        public List<PersistedEvent> Events = new List<PersistedEvent>();

        public List<Action> Actions = new List<Action>();

        public Task AddAction(Action action)
        {
            Actions.Add(action);
            return Task.CompletedTask;
        }

        public Task Commit()
        {
            // Randomly throw to emulate when db throws.
            // throw new Exception("assfdaasfd");
            if (DateTime.Now.Second % 2 == 0) throw new Exception("Error saving to db");

            Actions.ForEach(a => a());

            return Task.CompletedTask;
        }
    }
}