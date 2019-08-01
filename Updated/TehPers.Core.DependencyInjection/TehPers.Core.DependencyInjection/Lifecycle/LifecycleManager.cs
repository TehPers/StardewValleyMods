using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Syntax;
using StardewModdingAPI;

namespace TehPers.Core.DependencyInjection.Lifecycle
{
    internal sealed partial class LifecycleManager
    {
        private readonly IResolutionRoot _container;
        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;

        public LifecycleManager(IResolutionRoot container, IModHelper helper, IMonitor monitor)
        {
            this._container = container;
            this._monitor = monitor;
            this._helper = helper;
        }

        public void RegisterEvents()
        {
            this.RegisterEventsInternal();
        }

        private void HandleEvent<T>(string eventName, Action<T> callHandler)
        {
            List<Exception> eventExceptions = new List<Exception>();
            foreach (T handler in this._container.GetAll<T>())
            {
                try
                {
                    callHandler(handler);
                }
                catch (Exception ex)
                {
                    eventExceptions.Add(ex);
                }
            }

            if (eventExceptions.Any())
            {
                throw new AggregateException($"One or more exceptions occurred while handing {eventName}", eventExceptions);
            }
        }
    }
}
