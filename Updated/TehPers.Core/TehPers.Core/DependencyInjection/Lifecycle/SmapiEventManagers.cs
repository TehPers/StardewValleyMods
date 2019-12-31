using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.DependencyInjection.Lifecycle;

#pragma warning disable SA1403 // Auto-generated file, it's more easy to maintain if kept in one file.
#pragma warning disable SA1649 // Auto-generated file, it's more easy to maintain if kept in one file.
#pragma warning disable SA1505 // Auto-generated file, it's more difficult to remove extra blank lines.
namespace TehPers.Core.DependencyInjection.Lifecycle
{

    namespace GameLoop
    {

        internal class DayEndingEventManager : EventManager<DayEndingEventArgs>
        {
			private readonly IModHelper helper;

			public DayEndingEventManager(IModHelper helper, ISimpleFactory<IEventHandler<DayEndingEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.DayEnding += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.DayEnding -= this.HandleEvent;
			}
        }

        internal class OneSecondUpdateTickedEventManager : EventManager<OneSecondUpdateTickedEventArgs>
        {
			private readonly IModHelper helper;

			public OneSecondUpdateTickedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<OneSecondUpdateTickedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.OneSecondUpdateTicked += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.OneSecondUpdateTicked -= this.HandleEvent;
			}
        }

        internal class OneSecondUpdateTickingEventManager : EventManager<OneSecondUpdateTickingEventArgs>
        {
			private readonly IModHelper helper;

			public OneSecondUpdateTickingEventManager(IModHelper helper, ISimpleFactory<IEventHandler<OneSecondUpdateTickingEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.OneSecondUpdateTicking += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.OneSecondUpdateTicking -= this.HandleEvent;
			}
        }

        internal class ReturnedToTitleEventManager : EventManager<ReturnedToTitleEventArgs>
        {
			private readonly IModHelper helper;

			public ReturnedToTitleEventManager(IModHelper helper, ISimpleFactory<IEventHandler<ReturnedToTitleEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.ReturnedToTitle += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.ReturnedToTitle -= this.HandleEvent;
			}
        }

        internal class DayStartedEventManager : EventManager<DayStartedEventArgs>
        {
			private readonly IModHelper helper;

			public DayStartedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<DayStartedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.DayStarted += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.DayStarted -= this.HandleEvent;
			}
        }

        internal class SaveCreatedEventManager : EventManager<SaveCreatedEventArgs>
        {
			private readonly IModHelper helper;

			public SaveCreatedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<SaveCreatedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.SaveCreated += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.SaveCreated -= this.HandleEvent;
			}
        }

        internal class SaveCreatingEventManager : EventManager<SaveCreatingEventArgs>
        {
			private readonly IModHelper helper;

			public SaveCreatingEventManager(IModHelper helper, ISimpleFactory<IEventHandler<SaveCreatingEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.SaveCreating += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.SaveCreating -= this.HandleEvent;
			}
        }

        internal class SaveLoadedEventManager : EventManager<SaveLoadedEventArgs>
        {
			private readonly IModHelper helper;

			public SaveLoadedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<SaveLoadedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.SaveLoaded += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.SaveLoaded -= this.HandleEvent;
			}
        }

        internal class SavedEventManager : EventManager<SavedEventArgs>
        {
			private readonly IModHelper helper;

			public SavedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<SavedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.Saved += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.Saved -= this.HandleEvent;
			}
        }

        internal class SavingEventManager : EventManager<SavingEventArgs>
        {
			private readonly IModHelper helper;

			public SavingEventManager(IModHelper helper, ISimpleFactory<IEventHandler<SavingEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.Saving += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.Saving -= this.HandleEvent;
			}
        }

        internal class TimeChangedEventManager : EventManager<TimeChangedEventArgs>
        {
			private readonly IModHelper helper;

			public TimeChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<TimeChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.TimeChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.TimeChanged -= this.HandleEvent;
			}
        }

        internal class UpdateTickedEventManager : EventManager<UpdateTickedEventArgs>
        {
			private readonly IModHelper helper;

			public UpdateTickedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<UpdateTickedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.UpdateTicked += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.UpdateTicked -= this.HandleEvent;
			}
        }

        internal class UpdateTickingEventManager : EventManager<UpdateTickingEventArgs>
        {
			private readonly IModHelper helper;

			public UpdateTickingEventManager(IModHelper helper, ISimpleFactory<IEventHandler<UpdateTickingEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.GameLoop.UpdateTicking += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.GameLoop.UpdateTicking -= this.HandleEvent;
			}
        }
    }

    namespace Display
    {

        internal class MenuChangedEventManager : EventManager<MenuChangedEventArgs>
        {
			private readonly IModHelper helper;

			public MenuChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<MenuChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.MenuChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.MenuChanged -= this.HandleEvent;
			}
        }

        internal class RenderedEventManager : EventManager<RenderedEventArgs>
        {
			private readonly IModHelper helper;

			public RenderedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<RenderedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.Rendered += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.Rendered -= this.HandleEvent;
			}
        }

        internal class RenderedActiveMenuEventManager : EventManager<RenderedActiveMenuEventArgs>
        {
			private readonly IModHelper helper;

			public RenderedActiveMenuEventManager(IModHelper helper, ISimpleFactory<IEventHandler<RenderedActiveMenuEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.RenderedActiveMenu += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.RenderedActiveMenu -= this.HandleEvent;
			}
        }

        internal class RenderedHudEventManager : EventManager<RenderedHudEventArgs>
        {
			private readonly IModHelper helper;

			public RenderedHudEventManager(IModHelper helper, ISimpleFactory<IEventHandler<RenderedHudEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.RenderedHud += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.RenderedHud -= this.HandleEvent;
			}
        }

        internal class RenderedWorldEventManager : EventManager<RenderedWorldEventArgs>
        {
			private readonly IModHelper helper;

			public RenderedWorldEventManager(IModHelper helper, ISimpleFactory<IEventHandler<RenderedWorldEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.RenderedWorld += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.RenderedWorld -= this.HandleEvent;
			}
        }

        internal class RenderingEventManager : EventManager<RenderingEventArgs>
        {
			private readonly IModHelper helper;

			public RenderingEventManager(IModHelper helper, ISimpleFactory<IEventHandler<RenderingEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.Rendering += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.Rendering -= this.HandleEvent;
			}
        }

        internal class RenderingActiveMenuEventManager : EventManager<RenderingActiveMenuEventArgs>
        {
			private readonly IModHelper helper;

			public RenderingActiveMenuEventManager(IModHelper helper, ISimpleFactory<IEventHandler<RenderingActiveMenuEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.RenderingActiveMenu += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.RenderingActiveMenu -= this.HandleEvent;
			}
        }

        internal class RenderingHudEventManager : EventManager<RenderingHudEventArgs>
        {
			private readonly IModHelper helper;

			public RenderingHudEventManager(IModHelper helper, ISimpleFactory<IEventHandler<RenderingHudEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.RenderingHud += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.RenderingHud -= this.HandleEvent;
			}
        }

        internal class RenderingWorldEventManager : EventManager<RenderingWorldEventArgs>
        {
			private readonly IModHelper helper;

			public RenderingWorldEventManager(IModHelper helper, ISimpleFactory<IEventHandler<RenderingWorldEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.RenderingWorld += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.RenderingWorld -= this.HandleEvent;
			}
        }

        internal class WindowResizedEventManager : EventManager<WindowResizedEventArgs>
        {
			private readonly IModHelper helper;

			public WindowResizedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<WindowResizedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Display.WindowResized += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Display.WindowResized -= this.HandleEvent;
			}
        }
    }

    namespace Input
    {

        internal class ButtonPressedEventManager : EventManager<ButtonPressedEventArgs>
        {
			private readonly IModHelper helper;

			public ButtonPressedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<ButtonPressedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Input.ButtonPressed += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Input.ButtonPressed -= this.HandleEvent;
			}
        }

        internal class ButtonReleasedEventManager : EventManager<ButtonReleasedEventArgs>
        {
			private readonly IModHelper helper;

			public ButtonReleasedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<ButtonReleasedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Input.ButtonReleased += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Input.ButtonReleased -= this.HandleEvent;
			}
        }

        internal class CursorMovedEventManager : EventManager<CursorMovedEventArgs>
        {
			private readonly IModHelper helper;

			public CursorMovedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<CursorMovedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Input.CursorMoved += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Input.CursorMoved -= this.HandleEvent;
			}
        }

        internal class MouseWheelScrolledEventManager : EventManager<MouseWheelScrolledEventArgs>
        {
			private readonly IModHelper helper;

			public MouseWheelScrolledEventManager(IModHelper helper, ISimpleFactory<IEventHandler<MouseWheelScrolledEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Input.MouseWheelScrolled += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Input.MouseWheelScrolled -= this.HandleEvent;
			}
        }
    }

    namespace Multiplayer
    {

        internal class ModMessageReceivedEventManager : EventManager<ModMessageReceivedEventArgs>
        {
			private readonly IModHelper helper;

			public ModMessageReceivedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<ModMessageReceivedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Multiplayer.ModMessageReceived += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Multiplayer.ModMessageReceived -= this.HandleEvent;
			}
        }

        internal class PeerContextReceivedEventManager : EventManager<PeerContextReceivedEventArgs>
        {
			private readonly IModHelper helper;

			public PeerContextReceivedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<PeerContextReceivedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Multiplayer.PeerContextReceived += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Multiplayer.PeerContextReceived -= this.HandleEvent;
			}
        }

        internal class PeerDisconnectedEventManager : EventManager<PeerDisconnectedEventArgs>
        {
			private readonly IModHelper helper;

			public PeerDisconnectedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<PeerDisconnectedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Multiplayer.PeerDisconnected += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Multiplayer.PeerDisconnected -= this.HandleEvent;
			}
        }
    }

    namespace Player
    {

        internal class InventoryChangedEventManager : EventManager<InventoryChangedEventArgs>
        {
			private readonly IModHelper helper;

			public InventoryChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<InventoryChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Player.InventoryChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Player.InventoryChanged -= this.HandleEvent;
			}
        }

        internal class LevelChangedEventManager : EventManager<LevelChangedEventArgs>
        {
			private readonly IModHelper helper;

			public LevelChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<LevelChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Player.LevelChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Player.LevelChanged -= this.HandleEvent;
			}
        }

        internal class WarpedEventManager : EventManager<WarpedEventArgs>
        {
			private readonly IModHelper helper;

			public WarpedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<WarpedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Player.Warped += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Player.Warped -= this.HandleEvent;
			}
        }
    }

    namespace Specialized
    {

        internal class LoadStageChangedEventManager : EventManager<LoadStageChangedEventArgs>
        {
			private readonly IModHelper helper;

			public LoadStageChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<LoadStageChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Specialized.LoadStageChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Specialized.LoadStageChanged -= this.HandleEvent;
			}
        }

        internal class UnvalidatedUpdateTickedEventManager : EventManager<UnvalidatedUpdateTickedEventArgs>
        {
			private readonly IModHelper helper;

			public UnvalidatedUpdateTickedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<UnvalidatedUpdateTickedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Specialized.UnvalidatedUpdateTicked += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Specialized.UnvalidatedUpdateTicked -= this.HandleEvent;
			}
        }

        internal class UnvalidatedUpdateTickingEventManager : EventManager<UnvalidatedUpdateTickingEventArgs>
        {
			private readonly IModHelper helper;

			public UnvalidatedUpdateTickingEventManager(IModHelper helper, ISimpleFactory<IEventHandler<UnvalidatedUpdateTickingEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.Specialized.UnvalidatedUpdateTicking += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.Specialized.UnvalidatedUpdateTicking -= this.HandleEvent;
			}
        }
    }

    namespace World
    {

        internal class BuildingListChangedEventManager : EventManager<BuildingListChangedEventArgs>
        {
			private readonly IModHelper helper;

			public BuildingListChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<BuildingListChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.World.BuildingListChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.World.BuildingListChanged -= this.HandleEvent;
			}
        }

        internal class DebrisListChangedEventManager : EventManager<DebrisListChangedEventArgs>
        {
			private readonly IModHelper helper;

			public DebrisListChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<DebrisListChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.World.DebrisListChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.World.DebrisListChanged -= this.HandleEvent;
			}
        }

        internal class LargeTerrainFeatureListChangedEventManager : EventManager<LargeTerrainFeatureListChangedEventArgs>
        {
			private readonly IModHelper helper;

			public LargeTerrainFeatureListChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<LargeTerrainFeatureListChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.World.LargeTerrainFeatureListChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.World.LargeTerrainFeatureListChanged -= this.HandleEvent;
			}
        }

        internal class LocationListChangedEventManager : EventManager<LocationListChangedEventArgs>
        {
			private readonly IModHelper helper;

			public LocationListChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<LocationListChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.World.LocationListChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.World.LocationListChanged -= this.HandleEvent;
			}
        }

        internal class NpcListChangedEventManager : EventManager<NpcListChangedEventArgs>
        {
			private readonly IModHelper helper;

			public NpcListChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<NpcListChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.World.NpcListChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.World.NpcListChanged -= this.HandleEvent;
			}
        }

        internal class ObjectListChangedEventManager : EventManager<ObjectListChangedEventArgs>
        {
			private readonly IModHelper helper;

			public ObjectListChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<ObjectListChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.World.ObjectListChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.World.ObjectListChanged -= this.HandleEvent;
			}
        }

        internal class TerrainFeatureListChangedEventManager : EventManager<TerrainFeatureListChangedEventArgs>
        {
			private readonly IModHelper helper;

			public TerrainFeatureListChangedEventManager(IModHelper helper, ISimpleFactory<IEventHandler<TerrainFeatureListChangedEventArgs>> handlers)
				: base(handlers)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			public override void StartListening()
			{
                this.helper.Events.World.TerrainFeatureListChanged += this.HandleEvent;
			}

			public override void StopListening()
			{
                this.helper.Events.World.TerrainFeatureListChanged -= this.HandleEvent;
			}
        }
    }
}
#pragma warning restore SA1505
#pragma warning restore SA1649
#pragma warning restore SA1403