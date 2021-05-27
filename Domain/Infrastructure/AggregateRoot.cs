using System;
using System.Collections.Generic;
using System.Linq;

namespace MyRental
{
    public abstract class AggregateRoot
    {
        public AggregateRoot(long version)
        {
            Version = version;
        }

        public Guid Id { get; protected set; }
        public long Version { get; protected set; } = InitalVersion;
        protected static long InitalVersion => -1;

        private readonly ICollection<IDomainEvent> _uncommittedEvents = new LinkedList<IDomainEvent>();

        public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

        public IEnumerable<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsEnumerable();

        protected void RaiseEvent(IDomainEvent ev)
        {
            _uncommittedEvents.Add(ev);
            UpdateInternalState(ev);
        }

        public abstract void UpdateInternalState(IDomainEvent ev);

        public void ApplyEvent(IDomainEvent ev)
        {
            Version = Version + 1;
            UpdateInternalState(ev);
        }
    }
}
