using System;
using System.Collections.Generic;

namespace MyRental
{
    public record Recording(Guid Id, string Name, string Artist, int Year);

    public class Database
    {
        public List<Recording> Recordings = new List<Recording>();

        public List<Action> Actions = new List<Action>();

        public void AddAction(Action action)
        {
            Actions.Add(action);
        }
    }
}