using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.Core.DependencyInjection.Api.Lifecycle.GameLoop;
using TehPers.Core.DependencyInjection.Api.Lifecycle.Display;
using TehPers.Core.DependencyInjection.Api.Lifecycle.Input;
using TehPers.Core.DependencyInjection.Api.Lifecycle.Multiplayer;
using TehPers.Core.DependencyInjection.Api.Lifecycle.Player;
using TehPers.Core.DependencyInjection.Api.Lifecycle.Specialised;
using TehPers.Core.DependencyInjection.Api.Lifecycle.World;

namespace TehPers.Core.DependencyInjection.Lifecycle
{
    internal sealed partial class LifecycleManager : IDisposable
    {
		public static Type[] LifecycleInterfaces = {
			typeof(IDayEndingHandler),
			typeof(IOneSecondUpdateTickedHandler),
			typeof(IOneSecondUpdateTickingHandler),
			typeof(IReturnedToTitleHandler),
			typeof(IDayStartedHandler),
			typeof(ISaveCreatedHandler),
			typeof(ISaveCreatingHandler),
			typeof(ISaveLoadedHandler),
			typeof(ISavedHandler),
			typeof(ISavingHandler),
			typeof(ITimeChangedHandler),
			typeof(IUpdateTickedHandler),
			typeof(IUpdateTickingHandler),
			typeof(IMenuChangedHandler),
			typeof(IRenderedHandler),
			typeof(IRenderedActiveMenuHandler),
			typeof(IRenderedHudHandler),
			typeof(IRenderedWorldHandler),
			typeof(IRenderingHandler),
			typeof(IRenderingActiveMenuHandler),
			typeof(IRenderingHudHandler),
			typeof(IRenderingWorldHandler),
			typeof(IWindowResizedHandler),
			typeof(IButtonPressedHandler),
			typeof(IButtonReleasedHandler),
			typeof(ICursorMovedHandler),
			typeof(IMouseWheelScrolledHandler),
			typeof(IModMessageReceivedHandler),
			typeof(IPeerContextReceivedHandler),
			typeof(IPeerDisconnectedHandler),
			typeof(IInventoryChangedHandler),
			typeof(ILevelChangedHandler),
			typeof(IWarpedHandler),
			typeof(ILoadStageChangedHandler),
			typeof(IUnvalidatedUpdateTickedHandler),
			typeof(IUnvalidatedUpdateTickingHandler),
			typeof(IBuildingListChangedHandler),
			typeof(IDebrisListChangedHandler),
			typeof(ILargeTerrainFeatureListChangedHandler),
			typeof(ILocationListChangedHandler),
			typeof(INpcListChangedHandler),
			typeof(IObjectListChangedHandler),
			typeof(ITerrainFeatureListChangedHandler),
		};

		private void RegisterEventsInternal() {

			// GameLoop
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.DayEnding)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.DayEnding += this.OnDayEnding;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.OneSecondUpdateTicked)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.OneSecondUpdateTicking)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.OneSecondUpdateTicking += this.OnOneSecondUpdateTicking;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.ReturnedToTitle)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.DayStarted)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.SaveCreated)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.SaveCreated += this.OnSaveCreated;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.SaveCreating)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.SaveCreating += this.OnSaveCreating;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.SaveLoaded)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.Saved)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.Saved += this.OnSaved;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.Saving)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.Saving += this.OnSaving;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.TimeChanged)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.UpdateTicked)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
			this._monitor.Log($"Registering '{nameof(IGameLoopEvents.UpdateTicking)}' handler", LogLevel.Trace);
			this._helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;

			// Display
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.MenuChanged)}' handler", LogLevel.Trace);
			this._helper.Events.Display.MenuChanged += this.OnMenuChanged;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.Rendered)}' handler", LogLevel.Trace);
			this._helper.Events.Display.Rendered += this.OnRendered;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.RenderedActiveMenu)}' handler", LogLevel.Trace);
			this._helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.RenderedHud)}' handler", LogLevel.Trace);
			this._helper.Events.Display.RenderedHud += this.OnRenderedHud;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.RenderedWorld)}' handler", LogLevel.Trace);
			this._helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.Rendering)}' handler", LogLevel.Trace);
			this._helper.Events.Display.Rendering += this.OnRendering;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.RenderingActiveMenu)}' handler", LogLevel.Trace);
			this._helper.Events.Display.RenderingActiveMenu += this.OnRenderingActiveMenu;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.RenderingHud)}' handler", LogLevel.Trace);
			this._helper.Events.Display.RenderingHud += this.OnRenderingHud;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.RenderingWorld)}' handler", LogLevel.Trace);
			this._helper.Events.Display.RenderingWorld += this.OnRenderingWorld;
			this._monitor.Log($"Registering '{nameof(IDisplayEvents.WindowResized)}' handler", LogLevel.Trace);
			this._helper.Events.Display.WindowResized += this.OnWindowResized;

			// Input
			this._monitor.Log($"Registering '{nameof(IInputEvents.ButtonPressed)}' handler", LogLevel.Trace);
			this._helper.Events.Input.ButtonPressed += this.OnButtonPressed;
			this._monitor.Log($"Registering '{nameof(IInputEvents.ButtonReleased)}' handler", LogLevel.Trace);
			this._helper.Events.Input.ButtonReleased += this.OnButtonReleased;
			this._monitor.Log($"Registering '{nameof(IInputEvents.CursorMoved)}' handler", LogLevel.Trace);
			this._helper.Events.Input.CursorMoved += this.OnCursorMoved;
			this._monitor.Log($"Registering '{nameof(IInputEvents.MouseWheelScrolled)}' handler", LogLevel.Trace);
			this._helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;

			// Multiplayer
			this._monitor.Log($"Registering '{nameof(IMultiplayerEvents.ModMessageReceived)}' handler", LogLevel.Trace);
			this._helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
			this._monitor.Log($"Registering '{nameof(IMultiplayerEvents.PeerContextReceived)}' handler", LogLevel.Trace);
			this._helper.Events.Multiplayer.PeerContextReceived += this.OnPeerContextReceived;
			this._monitor.Log($"Registering '{nameof(IMultiplayerEvents.PeerDisconnected)}' handler", LogLevel.Trace);
			this._helper.Events.Multiplayer.PeerDisconnected += this.OnPeerDisconnected;

			// Player
			this._monitor.Log($"Registering '{nameof(IPlayerEvents.InventoryChanged)}' handler", LogLevel.Trace);
			this._helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
			this._monitor.Log($"Registering '{nameof(IPlayerEvents.LevelChanged)}' handler", LogLevel.Trace);
			this._helper.Events.Player.LevelChanged += this.OnLevelChanged;
			this._monitor.Log($"Registering '{nameof(IPlayerEvents.Warped)}' handler", LogLevel.Trace);
			this._helper.Events.Player.Warped += this.OnWarped;

			// Specialised
			this._monitor.Log($"Registering '{nameof(ISpecialisedEvents.LoadStageChanged)}' handler", LogLevel.Trace);
			this._helper.Events.Specialised.LoadStageChanged += this.OnLoadStageChanged;
			this._monitor.Log($"Registering '{nameof(ISpecialisedEvents.UnvalidatedUpdateTicked)}' handler", LogLevel.Trace);
			this._helper.Events.Specialised.UnvalidatedUpdateTicked += this.OnUnvalidatedUpdateTicked;
			this._monitor.Log($"Registering '{nameof(ISpecialisedEvents.UnvalidatedUpdateTicking)}' handler", LogLevel.Trace);
			this._helper.Events.Specialised.UnvalidatedUpdateTicking += this.OnUnvalidatedUpdateTicking;

			// World
			this._monitor.Log($"Registering '{nameof(IWorldEvents.BuildingListChanged)}' handler", LogLevel.Trace);
			this._helper.Events.World.BuildingListChanged += this.OnBuildingListChanged;
			this._monitor.Log($"Registering '{nameof(IWorldEvents.DebrisListChanged)}' handler", LogLevel.Trace);
			this._helper.Events.World.DebrisListChanged += this.OnDebrisListChanged;
			this._monitor.Log($"Registering '{nameof(IWorldEvents.LargeTerrainFeatureListChanged)}' handler", LogLevel.Trace);
			this._helper.Events.World.LargeTerrainFeatureListChanged += this.OnLargeTerrainFeatureListChanged;
			this._monitor.Log($"Registering '{nameof(IWorldEvents.LocationListChanged)}' handler", LogLevel.Trace);
			this._helper.Events.World.LocationListChanged += this.OnLocationListChanged;
			this._monitor.Log($"Registering '{nameof(IWorldEvents.NpcListChanged)}' handler", LogLevel.Trace);
			this._helper.Events.World.NpcListChanged += this.OnNpcListChanged;
			this._monitor.Log($"Registering '{nameof(IWorldEvents.ObjectListChanged)}' handler", LogLevel.Trace);
			this._helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
			this._monitor.Log($"Registering '{nameof(IWorldEvents.TerrainFeatureListChanged)}' handler", LogLevel.Trace);
			this._helper.Events.World.TerrainFeatureListChanged += this.OnTerrainFeatureListChanged;
		}

		public void Dispose() {

			// GameLoop
			this._helper.Events.GameLoop.DayEnding -= this.OnDayEnding;
			this._helper.Events.GameLoop.OneSecondUpdateTicked -= this.OnOneSecondUpdateTicked;
			this._helper.Events.GameLoop.OneSecondUpdateTicking -= this.OnOneSecondUpdateTicking;
			this._helper.Events.GameLoop.ReturnedToTitle -= this.OnReturnedToTitle;
			this._helper.Events.GameLoop.DayStarted -= this.OnDayStarted;
			this._helper.Events.GameLoop.SaveCreated -= this.OnSaveCreated;
			this._helper.Events.GameLoop.SaveCreating -= this.OnSaveCreating;
			this._helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
			this._helper.Events.GameLoop.Saved -= this.OnSaved;
			this._helper.Events.GameLoop.Saving -= this.OnSaving;
			this._helper.Events.GameLoop.TimeChanged -= this.OnTimeChanged;
			this._helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
			this._helper.Events.GameLoop.UpdateTicking -= this.OnUpdateTicking;

			// Display
			this._helper.Events.Display.MenuChanged -= this.OnMenuChanged;
			this._helper.Events.Display.Rendered -= this.OnRendered;
			this._helper.Events.Display.RenderedActiveMenu -= this.OnRenderedActiveMenu;
			this._helper.Events.Display.RenderedHud -= this.OnRenderedHud;
			this._helper.Events.Display.RenderedWorld -= this.OnRenderedWorld;
			this._helper.Events.Display.Rendering -= this.OnRendering;
			this._helper.Events.Display.RenderingActiveMenu -= this.OnRenderingActiveMenu;
			this._helper.Events.Display.RenderingHud -= this.OnRenderingHud;
			this._helper.Events.Display.RenderingWorld -= this.OnRenderingWorld;
			this._helper.Events.Display.WindowResized -= this.OnWindowResized;

			// Input
			this._helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
			this._helper.Events.Input.ButtonReleased -= this.OnButtonReleased;
			this._helper.Events.Input.CursorMoved -= this.OnCursorMoved;
			this._helper.Events.Input.MouseWheelScrolled -= this.OnMouseWheelScrolled;

			// Multiplayer
			this._helper.Events.Multiplayer.ModMessageReceived -= this.OnModMessageReceived;
			this._helper.Events.Multiplayer.PeerContextReceived -= this.OnPeerContextReceived;
			this._helper.Events.Multiplayer.PeerDisconnected -= this.OnPeerDisconnected;

			// Player
			this._helper.Events.Player.InventoryChanged -= this.OnInventoryChanged;
			this._helper.Events.Player.LevelChanged -= this.OnLevelChanged;
			this._helper.Events.Player.Warped -= this.OnWarped;

			// Specialised
			this._helper.Events.Specialised.LoadStageChanged -= this.OnLoadStageChanged;
			this._helper.Events.Specialised.UnvalidatedUpdateTicked -= this.OnUnvalidatedUpdateTicked;
			this._helper.Events.Specialised.UnvalidatedUpdateTicking -= this.OnUnvalidatedUpdateTicking;

			// World
			this._helper.Events.World.BuildingListChanged -= this.OnBuildingListChanged;
			this._helper.Events.World.DebrisListChanged -= this.OnDebrisListChanged;
			this._helper.Events.World.LargeTerrainFeatureListChanged -= this.OnLargeTerrainFeatureListChanged;
			this._helper.Events.World.LocationListChanged -= this.OnLocationListChanged;
			this._helper.Events.World.NpcListChanged -= this.OnNpcListChanged;
			this._helper.Events.World.ObjectListChanged -= this.OnObjectListChanged;
			this._helper.Events.World.TerrainFeatureListChanged -= this.OnTerrainFeatureListChanged;
		}

		#region GameLoop Event Handlers

		private void OnDayEnding(object sender, DayEndingEventArgs args) {
			this.HandleEvent<IDayEndingHandler>(nameof(IGameLoopEvents.DayEnding), handler => handler.OnDayEnding(sender, args));
		}

		private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs args) {
			this.HandleEvent<IOneSecondUpdateTickedHandler>(nameof(IGameLoopEvents.OneSecondUpdateTicked), handler => handler.OnOneSecondUpdateTicked(sender, args));
		}

		private void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs args) {
			this.HandleEvent<IOneSecondUpdateTickingHandler>(nameof(IGameLoopEvents.OneSecondUpdateTicking), handler => handler.OnOneSecondUpdateTicking(sender, args));
		}

		private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs args) {
			this.HandleEvent<IReturnedToTitleHandler>(nameof(IGameLoopEvents.ReturnedToTitle), handler => handler.OnReturnedToTitle(sender, args));
		}

		private void OnDayStarted(object sender, DayStartedEventArgs args) {
			this.HandleEvent<IDayStartedHandler>(nameof(IGameLoopEvents.DayStarted), handler => handler.OnDayStarted(sender, args));
		}

		private void OnSaveCreated(object sender, SaveCreatedEventArgs args) {
			this.HandleEvent<ISaveCreatedHandler>(nameof(IGameLoopEvents.SaveCreated), handler => handler.OnSaveCreated(sender, args));
		}

		private void OnSaveCreating(object sender, SaveCreatingEventArgs args) {
			this.HandleEvent<ISaveCreatingHandler>(nameof(IGameLoopEvents.SaveCreating), handler => handler.OnSaveCreating(sender, args));
		}

		private void OnSaveLoaded(object sender, SaveLoadedEventArgs args) {
			this.HandleEvent<ISaveLoadedHandler>(nameof(IGameLoopEvents.SaveLoaded), handler => handler.OnSaveLoaded(sender, args));
		}

		private void OnSaved(object sender, SavedEventArgs args) {
			this.HandleEvent<ISavedHandler>(nameof(IGameLoopEvents.Saved), handler => handler.OnSaved(sender, args));
		}

		private void OnSaving(object sender, SavingEventArgs args) {
			this.HandleEvent<ISavingHandler>(nameof(IGameLoopEvents.Saving), handler => handler.OnSaving(sender, args));
		}

		private void OnTimeChanged(object sender, TimeChangedEventArgs args) {
			this.HandleEvent<ITimeChangedHandler>(nameof(IGameLoopEvents.TimeChanged), handler => handler.OnTimeChanged(sender, args));
		}

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs args) {
			this.HandleEvent<IUpdateTickedHandler>(nameof(IGameLoopEvents.UpdateTicked), handler => handler.OnUpdateTicked(sender, args));
		}

		private void OnUpdateTicking(object sender, UpdateTickingEventArgs args) {
			this.HandleEvent<IUpdateTickingHandler>(nameof(IGameLoopEvents.UpdateTicking), handler => handler.OnUpdateTicking(sender, args));
		}
		#endregion

		#region Display Event Handlers

		private void OnMenuChanged(object sender, MenuChangedEventArgs args) {
			this.HandleEvent<IMenuChangedHandler>(nameof(IDisplayEvents.MenuChanged), handler => handler.OnMenuChanged(sender, args));
		}

		private void OnRendered(object sender, RenderedEventArgs args) {
			this.HandleEvent<IRenderedHandler>(nameof(IDisplayEvents.Rendered), handler => handler.OnRendered(sender, args));
		}

		private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs args) {
			this.HandleEvent<IRenderedActiveMenuHandler>(nameof(IDisplayEvents.RenderedActiveMenu), handler => handler.OnRenderedActiveMenu(sender, args));
		}

		private void OnRenderedHud(object sender, RenderedHudEventArgs args) {
			this.HandleEvent<IRenderedHudHandler>(nameof(IDisplayEvents.RenderedHud), handler => handler.OnRenderedHud(sender, args));
		}

		private void OnRenderedWorld(object sender, RenderedWorldEventArgs args) {
			this.HandleEvent<IRenderedWorldHandler>(nameof(IDisplayEvents.RenderedWorld), handler => handler.OnRenderedWorld(sender, args));
		}

		private void OnRendering(object sender, RenderingEventArgs args) {
			this.HandleEvent<IRenderingHandler>(nameof(IDisplayEvents.Rendering), handler => handler.OnRendering(sender, args));
		}

		private void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs args) {
			this.HandleEvent<IRenderingActiveMenuHandler>(nameof(IDisplayEvents.RenderingActiveMenu), handler => handler.OnRenderingActiveMenu(sender, args));
		}

		private void OnRenderingHud(object sender, RenderingHudEventArgs args) {
			this.HandleEvent<IRenderingHudHandler>(nameof(IDisplayEvents.RenderingHud), handler => handler.OnRenderingHud(sender, args));
		}

		private void OnRenderingWorld(object sender, RenderingWorldEventArgs args) {
			this.HandleEvent<IRenderingWorldHandler>(nameof(IDisplayEvents.RenderingWorld), handler => handler.OnRenderingWorld(sender, args));
		}

		private void OnWindowResized(object sender, WindowResizedEventArgs args) {
			this.HandleEvent<IWindowResizedHandler>(nameof(IDisplayEvents.WindowResized), handler => handler.OnWindowResized(sender, args));
		}
		#endregion

		#region Input Event Handlers

		private void OnButtonPressed(object sender, ButtonPressedEventArgs args) {
			this.HandleEvent<IButtonPressedHandler>(nameof(IInputEvents.ButtonPressed), handler => handler.OnButtonPressed(sender, args));
		}

		private void OnButtonReleased(object sender, ButtonReleasedEventArgs args) {
			this.HandleEvent<IButtonReleasedHandler>(nameof(IInputEvents.ButtonReleased), handler => handler.OnButtonReleased(sender, args));
		}

		private void OnCursorMoved(object sender, CursorMovedEventArgs args) {
			this.HandleEvent<ICursorMovedHandler>(nameof(IInputEvents.CursorMoved), handler => handler.OnCursorMoved(sender, args));
		}

		private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs args) {
			this.HandleEvent<IMouseWheelScrolledHandler>(nameof(IInputEvents.MouseWheelScrolled), handler => handler.OnMouseWheelScrolled(sender, args));
		}
		#endregion

		#region Multiplayer Event Handlers

		private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs args) {
			this.HandleEvent<IModMessageReceivedHandler>(nameof(IMultiplayerEvents.ModMessageReceived), handler => handler.OnModMessageReceived(sender, args));
		}

		private void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs args) {
			this.HandleEvent<IPeerContextReceivedHandler>(nameof(IMultiplayerEvents.PeerContextReceived), handler => handler.OnPeerContextReceived(sender, args));
		}

		private void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs args) {
			this.HandleEvent<IPeerDisconnectedHandler>(nameof(IMultiplayerEvents.PeerDisconnected), handler => handler.OnPeerDisconnected(sender, args));
		}
		#endregion

		#region Player Event Handlers

		private void OnInventoryChanged(object sender, InventoryChangedEventArgs args) {
			this.HandleEvent<IInventoryChangedHandler>(nameof(IPlayerEvents.InventoryChanged), handler => handler.OnInventoryChanged(sender, args));
		}

		private void OnLevelChanged(object sender, LevelChangedEventArgs args) {
			this.HandleEvent<ILevelChangedHandler>(nameof(IPlayerEvents.LevelChanged), handler => handler.OnLevelChanged(sender, args));
		}

		private void OnWarped(object sender, WarpedEventArgs args) {
			this.HandleEvent<IWarpedHandler>(nameof(IPlayerEvents.Warped), handler => handler.OnWarped(sender, args));
		}
		#endregion

		#region Specialised Event Handlers

		private void OnLoadStageChanged(object sender, LoadStageChangedEventArgs args) {
			this.HandleEvent<ILoadStageChangedHandler>(nameof(ISpecialisedEvents.LoadStageChanged), handler => handler.OnLoadStageChanged(sender, args));
		}

		private void OnUnvalidatedUpdateTicked(object sender, UnvalidatedUpdateTickedEventArgs args) {
			this.HandleEvent<IUnvalidatedUpdateTickedHandler>(nameof(ISpecialisedEvents.UnvalidatedUpdateTicked), handler => handler.OnUnvalidatedUpdateTicked(sender, args));
		}

		private void OnUnvalidatedUpdateTicking(object sender, UnvalidatedUpdateTickingEventArgs args) {
			this.HandleEvent<IUnvalidatedUpdateTickingHandler>(nameof(ISpecialisedEvents.UnvalidatedUpdateTicking), handler => handler.OnUnvalidatedUpdateTicking(sender, args));
		}
		#endregion

		#region World Event Handlers

		private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs args) {
			this.HandleEvent<IBuildingListChangedHandler>(nameof(IWorldEvents.BuildingListChanged), handler => handler.OnBuildingListChanged(sender, args));
		}

		private void OnDebrisListChanged(object sender, DebrisListChangedEventArgs args) {
			this.HandleEvent<IDebrisListChangedHandler>(nameof(IWorldEvents.DebrisListChanged), handler => handler.OnDebrisListChanged(sender, args));
		}

		private void OnLargeTerrainFeatureListChanged(object sender, LargeTerrainFeatureListChangedEventArgs args) {
			this.HandleEvent<ILargeTerrainFeatureListChangedHandler>(nameof(IWorldEvents.LargeTerrainFeatureListChanged), handler => handler.OnLargeTerrainFeatureListChanged(sender, args));
		}

		private void OnLocationListChanged(object sender, LocationListChangedEventArgs args) {
			this.HandleEvent<ILocationListChangedHandler>(nameof(IWorldEvents.LocationListChanged), handler => handler.OnLocationListChanged(sender, args));
		}

		private void OnNpcListChanged(object sender, NpcListChangedEventArgs args) {
			this.HandleEvent<INpcListChangedHandler>(nameof(IWorldEvents.NpcListChanged), handler => handler.OnNpcListChanged(sender, args));
		}

		private void OnObjectListChanged(object sender, ObjectListChangedEventArgs args) {
			this.HandleEvent<IObjectListChangedHandler>(nameof(IWorldEvents.ObjectListChanged), handler => handler.OnObjectListChanged(sender, args));
		}

		private void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs args) {
			this.HandleEvent<ITerrainFeatureListChangedHandler>(nameof(IWorldEvents.TerrainFeatureListChanged), handler => handler.OnTerrainFeatureListChanged(sender, args));
		}
		#endregion
	}
}