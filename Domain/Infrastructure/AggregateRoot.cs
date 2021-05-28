using System;
using System.Collections.Generic;
using System.Linq;

namespace MyRental
{
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set; }
        public long Version { get; protected set; } = InitalVersion;
        protected static long InitalVersion => -1;
        private readonly ICollection<IDomainEvent> _uncommittedEvents = new LinkedList<IDomainEvent>();

        protected AggregateRoot(long version)
        {
            Version = version;
        }

        public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

        public IEnumerable<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsEnumerable();

        protected void RaiseEvent(IDomainEvent ev)
        {
            _uncommittedEvents.Add(ev);
            ApplyEvent(ev);
        }

        protected abstract void ApplyEvent(IDomainEvent ev);

    }
}
