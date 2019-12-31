using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.DependencyInjection.Lifecycle
{
    internal static class LifecycleBindingExtensions
    {
		public static void BindManagedSmapiEvents(this IModKernel kernel)
		{

			// GameLoop
			kernel.Bind<IEventManager>().To<GameLoop.DayEndingEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.OneSecondUpdateTickedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.OneSecondUpdateTickingEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.ReturnedToTitleEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.DayStartedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.SaveCreatedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.SaveCreatingEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.SaveLoadedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.SavedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.SavingEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.TimeChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.UpdateTickedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<GameLoop.UpdateTickingEventManager>().InSingletonScope();

			// Display
			kernel.Bind<IEventManager>().To<Display.MenuChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.RenderedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.RenderedActiveMenuEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.RenderedHudEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.RenderedWorldEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.RenderingEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.RenderingActiveMenuEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.RenderingHudEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.RenderingWorldEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Display.WindowResizedEventManager>().InSingletonScope();

			// Input
			kernel.Bind<IEventManager>().To<Input.ButtonPressedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Input.ButtonReleasedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Input.CursorMovedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Input.MouseWheelScrolledEventManager>().InSingletonScope();

			// Multiplayer
			kernel.Bind<IEventManager>().To<Multiplayer.ModMessageReceivedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Multiplayer.PeerContextReceivedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Multiplayer.PeerDisconnectedEventManager>().InSingletonScope();

			// Player
			kernel.Bind<IEventManager>().To<Player.InventoryChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Player.LevelChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Player.WarpedEventManager>().InSingletonScope();

			// Specialized
			kernel.Bind<IEventManager>().To<Specialized.LoadStageChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Specialized.UnvalidatedUpdateTickedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<Specialized.UnvalidatedUpdateTickingEventManager>().InSingletonScope();

			// World
			kernel.Bind<IEventManager>().To<World.BuildingListChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<World.DebrisListChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<World.LargeTerrainFeatureListChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<World.LocationListChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<World.NpcListChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<World.ObjectListChangedEventManager>().InSingletonScope();
			kernel.Bind<IEventManager>().To<World.TerrainFeatureListChangedEventManager>().InSingletonScope();
		}
	}
}