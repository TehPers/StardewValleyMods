using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.DependencyInjection.Lifecycle
{
    internal static class LifecycleBindingExtensions
    {
		public static void BindManagedSmapiEvents(this IModKernel kernel)
		{

			// GameLoop
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.DayEndingEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.OneSecondUpdateTickedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.OneSecondUpdateTickingEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.ReturnedToTitleEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.DayStartedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.SaveCreatedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.SaveCreatingEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.SaveLoadedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.SavedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.SavingEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.TimeChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.UpdateTickedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.GameLoop.UpdateTickingEventManager>();

			// Display
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.MenuChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.RenderedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.RenderedActiveMenuEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.RenderedHudEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.RenderedWorldEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.RenderingEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.RenderingActiveMenuEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.RenderingHudEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.RenderingWorldEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Display.WindowResizedEventManager>();

			// Input
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Input.ButtonPressedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Input.ButtonReleasedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Input.CursorMovedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Input.MouseWheelScrolledEventManager>();

			// Multiplayer
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Multiplayer.ModMessageReceivedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Multiplayer.PeerContextReceivedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Multiplayer.PeerDisconnectedEventManager>();

			// Player
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Player.InventoryChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Player.LevelChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Player.WarpedEventManager>();

			// Specialized
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Specialized.LoadStageChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Specialized.UnvalidatedUpdateTickedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.Specialized.UnvalidatedUpdateTickingEventManager>();

			// World
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.World.BuildingListChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.World.DebrisListChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.World.LargeTerrainFeatureListChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.World.LocationListChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.World.NpcListChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.World.ObjectListChangedEventManager>();
			kernel.BindEventManager<TehPers.Core.DependencyInjection.Lifecycle.World.TerrainFeatureListChangedEventManager>();
		}
	}
}