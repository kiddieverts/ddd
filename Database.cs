using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRental
{
    public record Recording(Guid Id, string Name, string Artist, int Year);

    public class Database
    {
        public List<Recording> Recordings = new List<Recording>();

        public List<Action> Actions = new List<Action>();

        public Task AddAction(Action action)
        {
            Actions.Add(action);
            return Task.CompletedTask;
        }

        public Task Commit()
        {
            Actions.ForEach(a => a());
            return Task.CompletedTask;
        }
    }
}