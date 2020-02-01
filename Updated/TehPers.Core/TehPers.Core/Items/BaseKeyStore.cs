using System.Collections.Generic;
using StardewModdingAPI.Events;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.DependencyInjection.Lifecycle;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Items
{
    public abstract class BaseKeyStore : IEventHandler<SaveLoadedEventArgs>, IEventHandler<DayStartedEventArgs>
    {
        protected IDataStore<Dictionary<NamespacedId, int>> IndexStore { get; }

        protected BaseKeyStore(IDataStore<Dictionary<NamespacedId, int>> indexStore)
        {
            this.IndexStore = indexStore;
        }

        public bool TryGetIndex(NamespacedId id, out int index)
        {
            if (this.IndexStore.Access(data => data.TryGetValue(id)) is (true, var i))
            {
                index = i;
                return true;
            }

            index = default;
            return false;
        }

        void IEventHandler<SaveLoadedEventArgs>.HandleEvent(object sender, SaveLoadedEventArgs args)
        {
            this.Update();
        }

        void IEventHandler<DayStartedEventArgs>.HandleEvent(object sender, DayStartedEventArgs args)
        {
            this.Update();
        }

        private void Update()
        {
            this.IndexStore.Replace(_ => this.ConstructIdDictionary());
        }

        protected abstract Dictionary<NamespacedId, int> ConstructIdDictionary();
    }
}