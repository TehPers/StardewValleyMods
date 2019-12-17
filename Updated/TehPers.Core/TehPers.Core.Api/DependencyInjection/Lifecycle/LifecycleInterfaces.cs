using StardewModdingAPI.Events;

#pragma warning disable SA1403 // Auto-generated file, it's more easy to maintain if kept in one file.
#pragma warning disable SA1649 // Auto-generated file, it's more easy to maintain if kept in one file.
#pragma warning disable SA1505 // Auto-generated file, it's more difficult to remove extra blank lines.
namespace TehPers.Core.Api.DependencyInjection.Lifecycle
{

    namespace GameLoop
    {

        /// <summary>Handles events from <see cref="IGameLoopEvents.DayEnding" />.</summary>
        public interface IDayEndingHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.DayEnding" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnDayEnding(object sender, DayEndingEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.OneSecondUpdateTicked" />.</summary>
        public interface IOneSecondUpdateTickedHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.OneSecondUpdateTicked" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.OneSecondUpdateTicking" />.</summary>
        public interface IOneSecondUpdateTickingHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.OneSecondUpdateTicking" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.ReturnedToTitle" />.</summary>
        public interface IReturnedToTitleHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.ReturnedToTitle" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.DayStarted" />.</summary>
        public interface IDayStartedHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.DayStarted" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnDayStarted(object sender, DayStartedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.SaveCreated" />.</summary>
        public interface ISaveCreatedHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.SaveCreated" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnSaveCreated(object sender, SaveCreatedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.SaveCreating" />.</summary>
        public interface ISaveCreatingHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.SaveCreating" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnSaveCreating(object sender, SaveCreatingEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.SaveLoaded" />.</summary>
        public interface ISaveLoadedHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.SaveLoaded" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnSaveLoaded(object sender, SaveLoadedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.Saved" />.</summary>
        public interface ISavedHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.Saved" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnSaved(object sender, SavedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.Saving" />.</summary>
        public interface ISavingHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.Saving" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnSaving(object sender, SavingEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.TimeChanged" />.</summary>
        public interface ITimeChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.TimeChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnTimeChanged(object sender, TimeChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.UpdateTicked" />.</summary>
        public interface IUpdateTickedHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.UpdateTicked" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnUpdateTicked(object sender, UpdateTickedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.UpdateTicking" />.</summary>
        public interface IUpdateTickingHandler
        {
            /// <summary>Invoked whenever <see cref="IGameLoopEvents.UpdateTicking" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnUpdateTicking(object sender, UpdateTickingEventArgs args);
        }
    }

    namespace Display
    {

        /// <summary>Handles events from <see cref="IDisplayEvents.MenuChanged" />.</summary>
        public interface IMenuChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.MenuChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnMenuChanged(object sender, MenuChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.Rendered" />.</summary>
        public interface IRenderedHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.Rendered" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnRendered(object sender, RenderedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderedActiveMenu" />.</summary>
        public interface IRenderedActiveMenuHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.RenderedActiveMenu" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderedHud" />.</summary>
        public interface IRenderedHudHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.RenderedHud" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnRenderedHud(object sender, RenderedHudEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderedWorld" />.</summary>
        public interface IRenderedWorldHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.RenderedWorld" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnRenderedWorld(object sender, RenderedWorldEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.Rendering" />.</summary>
        public interface IRenderingHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.Rendering" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnRendering(object sender, RenderingEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderingActiveMenu" />.</summary>
        public interface IRenderingActiveMenuHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.RenderingActiveMenu" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderingHud" />.</summary>
        public interface IRenderingHudHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.RenderingHud" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnRenderingHud(object sender, RenderingHudEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderingWorld" />.</summary>
        public interface IRenderingWorldHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.RenderingWorld" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnRenderingWorld(object sender, RenderingWorldEventArgs args);
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.WindowResized" />.</summary>
        public interface IWindowResizedHandler
        {
            /// <summary>Invoked whenever <see cref="IDisplayEvents.WindowResized" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnWindowResized(object sender, WindowResizedEventArgs args);
        }
    }

    namespace Input
    {

        /// <summary>Handles events from <see cref="IInputEvents.ButtonPressed" />.</summary>
        public interface IButtonPressedHandler
        {
            /// <summary>Invoked whenever <see cref="IInputEvents.ButtonPressed" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnButtonPressed(object sender, ButtonPressedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IInputEvents.ButtonReleased" />.</summary>
        public interface IButtonReleasedHandler
        {
            /// <summary>Invoked whenever <see cref="IInputEvents.ButtonReleased" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnButtonReleased(object sender, ButtonReleasedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IInputEvents.CursorMoved" />.</summary>
        public interface ICursorMovedHandler
        {
            /// <summary>Invoked whenever <see cref="IInputEvents.CursorMoved" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnCursorMoved(object sender, CursorMovedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IInputEvents.MouseWheelScrolled" />.</summary>
        public interface IMouseWheelScrolledHandler
        {
            /// <summary>Invoked whenever <see cref="IInputEvents.MouseWheelScrolled" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs args);
        }
    }

    namespace Multiplayer
    {

        /// <summary>Handles events from <see cref="IMultiplayerEvents.ModMessageReceived" />.</summary>
        public interface IModMessageReceivedHandler
        {
            /// <summary>Invoked whenever <see cref="IMultiplayerEvents.ModMessageReceived" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnModMessageReceived(object sender, ModMessageReceivedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IMultiplayerEvents.PeerContextReceived" />.</summary>
        public interface IPeerContextReceivedHandler
        {
            /// <summary>Invoked whenever <see cref="IMultiplayerEvents.PeerContextReceived" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IMultiplayerEvents.PeerDisconnected" />.</summary>
        public interface IPeerDisconnectedHandler
        {
            /// <summary>Invoked whenever <see cref="IMultiplayerEvents.PeerDisconnected" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs args);
        }
    }

    namespace Player
    {

        /// <summary>Handles events from <see cref="IPlayerEvents.InventoryChanged" />.</summary>
        public interface IInventoryChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IPlayerEvents.InventoryChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnInventoryChanged(object sender, InventoryChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IPlayerEvents.LevelChanged" />.</summary>
        public interface ILevelChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IPlayerEvents.LevelChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnLevelChanged(object sender, LevelChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IPlayerEvents.Warped" />.</summary>
        public interface IWarpedHandler
        {
            /// <summary>Invoked whenever <see cref="IPlayerEvents.Warped" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnWarped(object sender, WarpedEventArgs args);
        }
    }

    namespace Specialized
    {

        /// <summary>Handles events from <see cref="ISpecializedEvents.LoadStageChanged" />.</summary>
        public interface ILoadStageChangedHandler
        {
            /// <summary>Invoked whenever <see cref="ISpecializedEvents.LoadStageChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnLoadStageChanged(object sender, LoadStageChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="ISpecializedEvents.UnvalidatedUpdateTicked" />.</summary>
        public interface IUnvalidatedUpdateTickedHandler
        {
            /// <summary>Invoked whenever <see cref="ISpecializedEvents.UnvalidatedUpdateTicked" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnUnvalidatedUpdateTicked(object sender, UnvalidatedUpdateTickedEventArgs args);
        }

        /// <summary>Handles events from <see cref="ISpecializedEvents.UnvalidatedUpdateTicking" />.</summary>
        public interface IUnvalidatedUpdateTickingHandler
        {
            /// <summary>Invoked whenever <see cref="ISpecializedEvents.UnvalidatedUpdateTicking" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnUnvalidatedUpdateTicking(object sender, UnvalidatedUpdateTickingEventArgs args);
        }
    }

    namespace World
    {

        /// <summary>Handles events from <see cref="IWorldEvents.BuildingListChanged" />.</summary>
        public interface IBuildingListChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IWorldEvents.BuildingListChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnBuildingListChanged(object sender, BuildingListChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IWorldEvents.DebrisListChanged" />.</summary>
        public interface IDebrisListChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IWorldEvents.DebrisListChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnDebrisListChanged(object sender, DebrisListChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IWorldEvents.LargeTerrainFeatureListChanged" />.</summary>
        public interface ILargeTerrainFeatureListChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IWorldEvents.LargeTerrainFeatureListChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnLargeTerrainFeatureListChanged(object sender, LargeTerrainFeatureListChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IWorldEvents.LocationListChanged" />.</summary>
        public interface ILocationListChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IWorldEvents.LocationListChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnLocationListChanged(object sender, LocationListChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IWorldEvents.NpcListChanged" />.</summary>
        public interface INpcListChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IWorldEvents.NpcListChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnNpcListChanged(object sender, NpcListChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IWorldEvents.ObjectListChanged" />.</summary>
        public interface IObjectListChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IWorldEvents.ObjectListChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnObjectListChanged(object sender, ObjectListChangedEventArgs args);
        }

        /// <summary>Handles events from <see cref="IWorldEvents.TerrainFeatureListChanged" />.</summary>
        public interface ITerrainFeatureListChangedHandler
        {
            /// <summary>Invoked whenever <see cref="IWorldEvents.TerrainFeatureListChanged" /> is invoked.</summary>
            /// <param name="sender">The object which invoked the event.</param>
            /// <param name="args">The arguments the event were invoked with.</param>
            void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs args);
        }
    }
}
#pragma warning restore SA1505
#pragma warning restore SA1649
#pragma warning restore SA1403