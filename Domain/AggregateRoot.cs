using System;
using System.Collections.Generic;
using System.Linq;

namespace MyRental
{
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set; }

        private readonly ICollection<IDomainEvent> _uncommittedEvents = new LinkedList<IDomainEvent>();

        public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

        public IEnumerable<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsEnumerable();

        protected void RaiseEvent<TEvent>(TEvent ev)
            where TEvent : IDomainEvent
        {
            _uncommittedEvents.Add(ev);
            ApplyEvent(ev);
        }

        protected abstract void ApplyEvent(IDomainEvent @event);
    }
}
