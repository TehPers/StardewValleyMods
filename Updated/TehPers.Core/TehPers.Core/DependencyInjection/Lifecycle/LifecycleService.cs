using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.DependencyInjection.Lifecycle
{
    internal class LifecycleService
    {
        private readonly IEventManager[] managedEvents;

        public LifecycleService(IEnumerable<IEventManager> managedEvents)
        {
            this.managedEvents = managedEvents.ToArray();
        }

        public void StartAll()
        {
            foreach (var managedEvent in this.managedEvents)
            {
                managedEvent.StartListening();
            }
        }

        public void StopAll()
        {
            foreach (var managedEvent in this.managedEvents)
            {
                managedEvent.StopListening();
            }
        }
    }
}
