using System;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.DependencyInjection.Lifecycle;
using TehPers.FishingFramework.Api.Events;

namespace TehPers.FishingFramework
{
    internal class FishingEventService
    {
        private readonly ISimpleFactory<IEventHandler<FishCatchingEventArgs>> fishCatchingHandlers;
        private readonly ISimpleFactory<IEventHandler<FishCaughtEventArgs>> fishCaughtHandlers;
        private readonly ISimpleFactory<IEventHandler<TreasureOpeningEventArgs>> treasureOpeningHandlers;
        private readonly ISimpleFactory<IEventHandler<TreasureOpenedEventArgs>> treasureOpenedHandlers;
        private readonly ISimpleFactory<IEventHandler<TrashCatchingEventArgs>> trashCatchingHandlers;
        private readonly ISimpleFactory<IEventHandler<TrashCaughtEventArgs>> trashCaughtHandlers;

        public FishingEventService(
            ISimpleFactory<IEventHandler<FishCatchingEventArgs>> fishCatchingHandlers,
            ISimpleFactory<IEventHandler<FishCaughtEventArgs>> fishCaughtHandlers,
            ISimpleFactory<IEventHandler<TreasureOpeningEventArgs>> treasureOpeningHandlers,
            ISimpleFactory<IEventHandler<TreasureOpenedEventArgs>> treasureOpenedHandlers,
            ISimpleFactory<IEventHandler<TrashCatchingEventArgs>> trashCatchingHandlers,
            ISimpleFactory<IEventHandler<TrashCaughtEventArgs>> trashCaughtHandlers)
        {
            this.fishCatchingHandlers = fishCatchingHandlers ?? throw new ArgumentNullException(nameof(fishCatchingHandlers));
            this.fishCaughtHandlers = fishCaughtHandlers ?? throw new ArgumentNullException(nameof(fishCaughtHandlers));
            this.treasureOpeningHandlers = treasureOpeningHandlers ?? throw new ArgumentNullException(nameof(treasureOpeningHandlers));
            this.treasureOpenedHandlers = treasureOpenedHandlers ?? throw new ArgumentNullException(nameof(treasureOpenedHandlers));
            this.trashCatchingHandlers = trashCatchingHandlers ?? throw new ArgumentNullException(nameof(trashCatchingHandlers));
            this.trashCaughtHandlers = trashCaughtHandlers ?? throw new ArgumentNullException(nameof(trashCaughtHandlers));
        }

        private static void HandleEvent<T>(object sender, T args, ISimpleFactory<IEventHandler<T>> handlers)
            where T : EventArgs
        {
            foreach (var handler in handlers.GetAll())
            {
                handler.HandleEvent(sender, args);
            }
        }

        public void OnFishCatching(object sender, FishCatchingEventArgs args)
        {
            FishingEventService.HandleEvent(sender, args, this.fishCatchingHandlers);
        }

        public void OnFishCaught(object sender, FishCaughtEventArgs args)
        {
            FishingEventService.HandleEvent(sender, args, this.fishCaughtHandlers);
        }

        public void OnTreasureOpening(object sender, TreasureOpeningEventArgs args)
        {
            FishingEventService.HandleEvent(sender, args, this.treasureOpeningHandlers);
        }

        public void OnTreasureOpened(object sender, TreasureOpenedEventArgs args)
        {
            FishingEventService.HandleEvent(sender, args, this.treasureOpenedHandlers);
        }

        public void OnTrashCatching(object sender, TrashCatchingEventArgs args)
        {
            FishingEventService.HandleEvent(sender, args, this.trashCatchingHandlers);
        }

        public void OnTrashCaught(object sender, TrashCaughtEventArgs args)
        {
            FishingEventService.HandleEvent(sender, args, this.trashCaughtHandlers);
        }
    }
}