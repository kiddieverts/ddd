using System;
using System.Collections.Generic;
using System.Linq;

namespace MyRental
{
    public class AggregateRoot
    {
        public Guid Id { get; protected set; }

        private readonly ICollection<IDomainEvent> _uncommittedEvents = new LinkedList<IDomainEvent>();

        public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

        public IEnumerable<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsEnumerable();

        protected void RaiseEvent<TEvent>(TEvent ev, Action<TEvent> fn)
            where TEvent : IDomainEvent
        {
            _uncommittedEvents.Add(ev);
            fn(ev);
        }
    }
}
