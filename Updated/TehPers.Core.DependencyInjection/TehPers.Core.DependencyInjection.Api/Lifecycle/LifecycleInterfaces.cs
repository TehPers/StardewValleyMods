using StardewModdingAPI.Events;

namespace TehPers.Core.DependencyInjection.Api.Lifecycle {

    namespace GameLoop {

        public interface IDayEndingHandler {
            void OnDayEnding(object sender, DayEndingEventArgs args);
        }

        public interface IOneSecondUpdateTickedHandler {
            void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs args);
        }

        public interface IOneSecondUpdateTickingHandler {
            void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs args);
        }

        public interface IReturnedToTitleHandler {
            void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs args);
        }

        public interface IDayStartedHandler {
            void OnDayStarted(object sender, DayStartedEventArgs args);
        }

        public interface ISaveCreatedHandler {
            void OnSaveCreated(object sender, SaveCreatedEventArgs args);
        }

        public interface ISaveCreatingHandler {
            void OnSaveCreating(object sender, SaveCreatingEventArgs args);
        }

        public interface ISaveLoadedHandler {
            void OnSaveLoaded(object sender, SaveLoadedEventArgs args);
        }

        public interface ISavedHandler {
            void OnSaved(object sender, SavedEventArgs args);
        }

        public interface ISavingHandler {
            void OnSaving(object sender, SavingEventArgs args);
        }

        public interface ITimeChangedHandler {
            void OnTimeChanged(object sender, TimeChangedEventArgs args);
        }

        public interface IUpdateTickedHandler {
            void OnUpdateTicked(object sender, UpdateTickedEventArgs args);
        }

        public interface IUpdateTickingHandler {
            void OnUpdateTicking(object sender, UpdateTickingEventArgs args);
        }
    }

    namespace Display {

        public interface IMenuChangedHandler {
            void OnMenuChanged(object sender, MenuChangedEventArgs args);
        }

        public interface IRenderedHandler {
            void OnRendered(object sender, RenderedEventArgs args);
        }

        public interface IRenderedActiveMenuHandler {
            void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs args);
        }

        public interface IRenderedHudHandler {
            void OnRenderedHud(object sender, RenderedHudEventArgs args);
        }

        public interface IRenderedWorldHandler {
            void OnRenderedWorld(object sender, RenderedWorldEventArgs args);
        }

        public interface IRenderingHandler {
            void OnRendering(object sender, RenderingEventArgs args);
        }

        public interface IRenderingActiveMenuHandler {
            void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs args);
        }

        public interface IRenderingHudHandler {
            void OnRenderingHud(object sender, RenderingHudEventArgs args);
        }

        public interface IRenderingWorldHandler {
            void OnRenderingWorld(object sender, RenderingWorldEventArgs args);
        }

        public interface IWindowResizedHandler {
            void OnWindowResized(object sender, WindowResizedEventArgs args);
        }
    }

    namespace Input {

        public interface IButtonPressedHandler {
            void OnButtonPressed(object sender, ButtonPressedEventArgs args);
        }

        public interface IButtonReleasedHandler {
            void OnButtonReleased(object sender, ButtonReleasedEventArgs args);
        }

        public interface ICursorMovedHandler {
            void OnCursorMoved(object sender, CursorMovedEventArgs args);
        }

        public interface IMouseWheelScrolledHandler {
            void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs args);
        }
    }

    namespace Multiplayer {

        public interface IModMessageReceivedHandler {
            void OnModMessageReceived(object sender, ModMessageReceivedEventArgs args);
        }

        public interface IPeerContextReceivedHandler {
            void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs args);
        }

        public interface IPeerDisconnectedHandler {
            void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs args);
        }
    }

    namespace Player {

        public interface IInventoryChangedHandler {
            void OnInventoryChanged(object sender, InventoryChangedEventArgs args);
        }

        public interface ILevelChangedHandler {
            void OnLevelChanged(object sender, LevelChangedEventArgs args);
        }

        public interface IWarpedHandler {
            void OnWarped(object sender, WarpedEventArgs args);
        }
    }

    namespace Specialised {

        public interface ILoadStageChangedHandler {
            void OnLoadStageChanged(object sender, LoadStageChangedEventArgs args);
        }

        public interface IUnvalidatedUpdateTickedHandler {
            void OnUnvalidatedUpdateTicked(object sender, UnvalidatedUpdateTickedEventArgs args);
        }

        public interface IUnvalidatedUpdateTickingHandler {
            void OnUnvalidatedUpdateTicking(object sender, UnvalidatedUpdateTickingEventArgs args);
        }
    }

    namespace World {

        public interface IBuildingListChangedHandler {
            void OnBuildingListChanged(object sender, BuildingListChangedEventArgs args);
        }

        public interface IDebrisListChangedHandler {
            void OnDebrisListChanged(object sender, DebrisListChangedEventArgs args);
        }

        public interface ILargeTerrainFeatureListChangedHandler {
            void OnLargeTerrainFeatureListChanged(object sender, LargeTerrainFeatureListChangedEventArgs args);
        }

        public interface ILocationListChangedHandler {
            void OnLocationListChanged(object sender, LocationListChangedEventArgs args);
        }

        public interface INpcListChangedHandler {
            void OnNpcListChanged(object sender, NpcListChangedEventArgs args);
        }

        public interface IObjectListChangedHandler {
            void OnObjectListChanged(object sender, ObjectListChangedEventArgs args);
        }

        public interface ITerrainFeatureListChangedHandler {
            void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs args);
        }
    }
}