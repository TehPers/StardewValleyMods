using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.DependencyInjection.Lifecycle;
using TehPers.Core.Api.DependencyInjection.Lifecycle.GameLoop;
using TehPers.Core.Api.DependencyInjection.Lifecycle.Display;
using TehPers.Core.Api.DependencyInjection.Lifecycle.Input;
using TehPers.Core.Api.DependencyInjection.Lifecycle.Multiplayer;
using TehPers.Core.Api.DependencyInjection.Lifecycle.Player;
using TehPers.Core.Api.DependencyInjection.Lifecycle.Specialized;
using TehPers.Core.Api.DependencyInjection.Lifecycle.World;

#pragma warning disable SA1403 // Auto-generated file, it's more easy to maintain if kept in one file.
#pragma warning disable SA1649 // Auto-generated file, it's more easy to maintain if kept in one file.
#pragma warning disable SA1505 // Auto-generated file, it's more difficult to remove extra blank lines.
namespace TehPers.Core.DependencyInjection.Lifecycle
{

    namespace GameLoop
    {

        /// <summary>Handles events from <see cref="IGameLoopEvents.DayEnding" />.</summary>
        internal class DayEndingEventManager : EventManager<IDayEndingHandler, DayEndingEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="DayEndingEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public DayEndingEventManager(IModHelper helper, ISimpleFactory<IDayEndingHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.DayEnding += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.DayEnding -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IDayEndingHandler handler, object sender, DayEndingEventArgs eventArgs)
			{
				handler.OnDayEnding(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.OneSecondUpdateTicked" />.</summary>
        internal class OneSecondUpdateTickedEventManager : EventManager<IOneSecondUpdateTickedHandler, OneSecondUpdateTickedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="OneSecondUpdateTickedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public OneSecondUpdateTickedEventManager(IModHelper helper, ISimpleFactory<IOneSecondUpdateTickedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.OneSecondUpdateTicked += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.OneSecondUpdateTicked -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IOneSecondUpdateTickedHandler handler, object sender, OneSecondUpdateTickedEventArgs eventArgs)
			{
				handler.OnOneSecondUpdateTicked(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.OneSecondUpdateTicking" />.</summary>
        internal class OneSecondUpdateTickingEventManager : EventManager<IOneSecondUpdateTickingHandler, OneSecondUpdateTickingEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="OneSecondUpdateTickingEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public OneSecondUpdateTickingEventManager(IModHelper helper, ISimpleFactory<IOneSecondUpdateTickingHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.OneSecondUpdateTicking += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.OneSecondUpdateTicking -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IOneSecondUpdateTickingHandler handler, object sender, OneSecondUpdateTickingEventArgs eventArgs)
			{
				handler.OnOneSecondUpdateTicking(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.ReturnedToTitle" />.</summary>
        internal class ReturnedToTitleEventManager : EventManager<IReturnedToTitleHandler, ReturnedToTitleEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="ReturnedToTitleEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public ReturnedToTitleEventManager(IModHelper helper, ISimpleFactory<IReturnedToTitleHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.ReturnedToTitle += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.ReturnedToTitle -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IReturnedToTitleHandler handler, object sender, ReturnedToTitleEventArgs eventArgs)
			{
				handler.OnReturnedToTitle(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.DayStarted" />.</summary>
        internal class DayStartedEventManager : EventManager<IDayStartedHandler, DayStartedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="DayStartedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public DayStartedEventManager(IModHelper helper, ISimpleFactory<IDayStartedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.DayStarted += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.DayStarted -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IDayStartedHandler handler, object sender, DayStartedEventArgs eventArgs)
			{
				handler.OnDayStarted(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.SaveCreated" />.</summary>
        internal class SaveCreatedEventManager : EventManager<ISaveCreatedHandler, SaveCreatedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="SaveCreatedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public SaveCreatedEventManager(IModHelper helper, ISimpleFactory<ISaveCreatedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.SaveCreated += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.SaveCreated -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ISaveCreatedHandler handler, object sender, SaveCreatedEventArgs eventArgs)
			{
				handler.OnSaveCreated(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.SaveCreating" />.</summary>
        internal class SaveCreatingEventManager : EventManager<ISaveCreatingHandler, SaveCreatingEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="SaveCreatingEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public SaveCreatingEventManager(IModHelper helper, ISimpleFactory<ISaveCreatingHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.SaveCreating += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.SaveCreating -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ISaveCreatingHandler handler, object sender, SaveCreatingEventArgs eventArgs)
			{
				handler.OnSaveCreating(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.SaveLoaded" />.</summary>
        internal class SaveLoadedEventManager : EventManager<ISaveLoadedHandler, SaveLoadedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="SaveLoadedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public SaveLoadedEventManager(IModHelper helper, ISimpleFactory<ISaveLoadedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.SaveLoaded += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.SaveLoaded -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ISaveLoadedHandler handler, object sender, SaveLoadedEventArgs eventArgs)
			{
				handler.OnSaveLoaded(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.Saved" />.</summary>
        internal class SavedEventManager : EventManager<ISavedHandler, SavedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="SavedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public SavedEventManager(IModHelper helper, ISimpleFactory<ISavedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.Saved += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.Saved -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ISavedHandler handler, object sender, SavedEventArgs eventArgs)
			{
				handler.OnSaved(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.Saving" />.</summary>
        internal class SavingEventManager : EventManager<ISavingHandler, SavingEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="SavingEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public SavingEventManager(IModHelper helper, ISimpleFactory<ISavingHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.Saving += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.Saving -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ISavingHandler handler, object sender, SavingEventArgs eventArgs)
			{
				handler.OnSaving(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.TimeChanged" />.</summary>
        internal class TimeChangedEventManager : EventManager<ITimeChangedHandler, TimeChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="TimeChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public TimeChangedEventManager(IModHelper helper, ISimpleFactory<ITimeChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.TimeChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.TimeChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ITimeChangedHandler handler, object sender, TimeChangedEventArgs eventArgs)
			{
				handler.OnTimeChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.UpdateTicked" />.</summary>
        internal class UpdateTickedEventManager : EventManager<IUpdateTickedHandler, UpdateTickedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="UpdateTickedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public UpdateTickedEventManager(IModHelper helper, ISimpleFactory<IUpdateTickedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.UpdateTicked += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.UpdateTicked -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IUpdateTickedHandler handler, object sender, UpdateTickedEventArgs eventArgs)
			{
				handler.OnUpdateTicked(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IGameLoopEvents.UpdateTicking" />.</summary>
        internal class UpdateTickingEventManager : EventManager<IUpdateTickingHandler, UpdateTickingEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="UpdateTickingEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public UpdateTickingEventManager(IModHelper helper, ISimpleFactory<IUpdateTickingHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.GameLoop.UpdateTicking += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.GameLoop.UpdateTicking -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IUpdateTickingHandler handler, object sender, UpdateTickingEventArgs eventArgs)
			{
				handler.OnUpdateTicking(sender, eventArgs);
			}
        }
    }

    namespace Display
    {

        /// <summary>Handles events from <see cref="IDisplayEvents.MenuChanged" />.</summary>
        internal class MenuChangedEventManager : EventManager<IMenuChangedHandler, MenuChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="MenuChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public MenuChangedEventManager(IModHelper helper, ISimpleFactory<IMenuChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.MenuChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.MenuChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IMenuChangedHandler handler, object sender, MenuChangedEventArgs eventArgs)
			{
				handler.OnMenuChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.Rendered" />.</summary>
        internal class RenderedEventManager : EventManager<IRenderedHandler, RenderedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public RenderedEventManager(IModHelper helper, ISimpleFactory<IRenderedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.Rendered += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.Rendered -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IRenderedHandler handler, object sender, RenderedEventArgs eventArgs)
			{
				handler.OnRendered(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderedActiveMenu" />.</summary>
        internal class RenderedActiveMenuEventManager : EventManager<IRenderedActiveMenuHandler, RenderedActiveMenuEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderedActiveMenuEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public RenderedActiveMenuEventManager(IModHelper helper, ISimpleFactory<IRenderedActiveMenuHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.RenderedActiveMenu += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.RenderedActiveMenu -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IRenderedActiveMenuHandler handler, object sender, RenderedActiveMenuEventArgs eventArgs)
			{
				handler.OnRenderedActiveMenu(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderedHud" />.</summary>
        internal class RenderedHudEventManager : EventManager<IRenderedHudHandler, RenderedHudEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderedHudEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public RenderedHudEventManager(IModHelper helper, ISimpleFactory<IRenderedHudHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.RenderedHud += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.RenderedHud -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IRenderedHudHandler handler, object sender, RenderedHudEventArgs eventArgs)
			{
				handler.OnRenderedHud(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderedWorld" />.</summary>
        internal class RenderedWorldEventManager : EventManager<IRenderedWorldHandler, RenderedWorldEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderedWorldEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public RenderedWorldEventManager(IModHelper helper, ISimpleFactory<IRenderedWorldHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.RenderedWorld += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.RenderedWorld -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IRenderedWorldHandler handler, object sender, RenderedWorldEventArgs eventArgs)
			{
				handler.OnRenderedWorld(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.Rendering" />.</summary>
        internal class RenderingEventManager : EventManager<IRenderingHandler, RenderingEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderingEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public RenderingEventManager(IModHelper helper, ISimpleFactory<IRenderingHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.Rendering += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.Rendering -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IRenderingHandler handler, object sender, RenderingEventArgs eventArgs)
			{
				handler.OnRendering(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderingActiveMenu" />.</summary>
        internal class RenderingActiveMenuEventManager : EventManager<IRenderingActiveMenuHandler, RenderingActiveMenuEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderingActiveMenuEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public RenderingActiveMenuEventManager(IModHelper helper, ISimpleFactory<IRenderingActiveMenuHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.RenderingActiveMenu += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.RenderingActiveMenu -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IRenderingActiveMenuHandler handler, object sender, RenderingActiveMenuEventArgs eventArgs)
			{
				handler.OnRenderingActiveMenu(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderingHud" />.</summary>
        internal class RenderingHudEventManager : EventManager<IRenderingHudHandler, RenderingHudEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderingHudEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public RenderingHudEventManager(IModHelper helper, ISimpleFactory<IRenderingHudHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.RenderingHud += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.RenderingHud -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IRenderingHudHandler handler, object sender, RenderingHudEventArgs eventArgs)
			{
				handler.OnRenderingHud(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.RenderingWorld" />.</summary>
        internal class RenderingWorldEventManager : EventManager<IRenderingWorldHandler, RenderingWorldEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderingWorldEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public RenderingWorldEventManager(IModHelper helper, ISimpleFactory<IRenderingWorldHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.RenderingWorld += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.RenderingWorld -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IRenderingWorldHandler handler, object sender, RenderingWorldEventArgs eventArgs)
			{
				handler.OnRenderingWorld(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IDisplayEvents.WindowResized" />.</summary>
        internal class WindowResizedEventManager : EventManager<IWindowResizedHandler, WindowResizedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="WindowResizedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public WindowResizedEventManager(IModHelper helper, ISimpleFactory<IWindowResizedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Display.WindowResized += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Display.WindowResized -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IWindowResizedHandler handler, object sender, WindowResizedEventArgs eventArgs)
			{
				handler.OnWindowResized(sender, eventArgs);
			}
        }
    }

    namespace Input
    {

        /// <summary>Handles events from <see cref="IInputEvents.ButtonPressed" />.</summary>
        internal class ButtonPressedEventManager : EventManager<IButtonPressedHandler, ButtonPressedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="ButtonPressedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public ButtonPressedEventManager(IModHelper helper, ISimpleFactory<IButtonPressedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Input.ButtonPressed += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Input.ButtonPressed -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IButtonPressedHandler handler, object sender, ButtonPressedEventArgs eventArgs)
			{
				handler.OnButtonPressed(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IInputEvents.ButtonReleased" />.</summary>
        internal class ButtonReleasedEventManager : EventManager<IButtonReleasedHandler, ButtonReleasedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="ButtonReleasedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public ButtonReleasedEventManager(IModHelper helper, ISimpleFactory<IButtonReleasedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Input.ButtonReleased += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Input.ButtonReleased -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IButtonReleasedHandler handler, object sender, ButtonReleasedEventArgs eventArgs)
			{
				handler.OnButtonReleased(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IInputEvents.CursorMoved" />.</summary>
        internal class CursorMovedEventManager : EventManager<ICursorMovedHandler, CursorMovedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="CursorMovedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public CursorMovedEventManager(IModHelper helper, ISimpleFactory<ICursorMovedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Input.CursorMoved += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Input.CursorMoved -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ICursorMovedHandler handler, object sender, CursorMovedEventArgs eventArgs)
			{
				handler.OnCursorMoved(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IInputEvents.MouseWheelScrolled" />.</summary>
        internal class MouseWheelScrolledEventManager : EventManager<IMouseWheelScrolledHandler, MouseWheelScrolledEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="MouseWheelScrolledEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public MouseWheelScrolledEventManager(IModHelper helper, ISimpleFactory<IMouseWheelScrolledHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Input.MouseWheelScrolled += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Input.MouseWheelScrolled -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IMouseWheelScrolledHandler handler, object sender, MouseWheelScrolledEventArgs eventArgs)
			{
				handler.OnMouseWheelScrolled(sender, eventArgs);
			}
        }
    }

    namespace Multiplayer
    {

        /// <summary>Handles events from <see cref="IMultiplayerEvents.ModMessageReceived" />.</summary>
        internal class ModMessageReceivedEventManager : EventManager<IModMessageReceivedHandler, ModMessageReceivedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="ModMessageReceivedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public ModMessageReceivedEventManager(IModHelper helper, ISimpleFactory<IModMessageReceivedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Multiplayer.ModMessageReceived += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Multiplayer.ModMessageReceived -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IModMessageReceivedHandler handler, object sender, ModMessageReceivedEventArgs eventArgs)
			{
				handler.OnModMessageReceived(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IMultiplayerEvents.PeerContextReceived" />.</summary>
        internal class PeerContextReceivedEventManager : EventManager<IPeerContextReceivedHandler, PeerContextReceivedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="PeerContextReceivedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public PeerContextReceivedEventManager(IModHelper helper, ISimpleFactory<IPeerContextReceivedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Multiplayer.PeerContextReceived += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Multiplayer.PeerContextReceived -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IPeerContextReceivedHandler handler, object sender, PeerContextReceivedEventArgs eventArgs)
			{
				handler.OnPeerContextReceived(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IMultiplayerEvents.PeerDisconnected" />.</summary>
        internal class PeerDisconnectedEventManager : EventManager<IPeerDisconnectedHandler, PeerDisconnectedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="PeerDisconnectedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public PeerDisconnectedEventManager(IModHelper helper, ISimpleFactory<IPeerDisconnectedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Multiplayer.PeerDisconnected += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Multiplayer.PeerDisconnected -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IPeerDisconnectedHandler handler, object sender, PeerDisconnectedEventArgs eventArgs)
			{
				handler.OnPeerDisconnected(sender, eventArgs);
			}
        }
    }

    namespace Player
    {

        /// <summary>Handles events from <see cref="IPlayerEvents.InventoryChanged" />.</summary>
        internal class InventoryChangedEventManager : EventManager<IInventoryChangedHandler, InventoryChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="InventoryChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public InventoryChangedEventManager(IModHelper helper, ISimpleFactory<IInventoryChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Player.InventoryChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Player.InventoryChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IInventoryChangedHandler handler, object sender, InventoryChangedEventArgs eventArgs)
			{
				handler.OnInventoryChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IPlayerEvents.LevelChanged" />.</summary>
        internal class LevelChangedEventManager : EventManager<ILevelChangedHandler, LevelChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="LevelChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public LevelChangedEventManager(IModHelper helper, ISimpleFactory<ILevelChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Player.LevelChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Player.LevelChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ILevelChangedHandler handler, object sender, LevelChangedEventArgs eventArgs)
			{
				handler.OnLevelChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IPlayerEvents.Warped" />.</summary>
        internal class WarpedEventManager : EventManager<IWarpedHandler, WarpedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="WarpedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public WarpedEventManager(IModHelper helper, ISimpleFactory<IWarpedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Player.Warped += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Player.Warped -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IWarpedHandler handler, object sender, WarpedEventArgs eventArgs)
			{
				handler.OnWarped(sender, eventArgs);
			}
        }
    }

    namespace Specialized
    {

        /// <summary>Handles events from <see cref="ISpecializedEvents.LoadStageChanged" />.</summary>
        internal class LoadStageChangedEventManager : EventManager<ILoadStageChangedHandler, LoadStageChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="LoadStageChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public LoadStageChangedEventManager(IModHelper helper, ISimpleFactory<ILoadStageChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Specialized.LoadStageChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Specialized.LoadStageChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ILoadStageChangedHandler handler, object sender, LoadStageChangedEventArgs eventArgs)
			{
				handler.OnLoadStageChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="ISpecializedEvents.UnvalidatedUpdateTicked" />.</summary>
        internal class UnvalidatedUpdateTickedEventManager : EventManager<IUnvalidatedUpdateTickedHandler, UnvalidatedUpdateTickedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="UnvalidatedUpdateTickedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public UnvalidatedUpdateTickedEventManager(IModHelper helper, ISimpleFactory<IUnvalidatedUpdateTickedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Specialized.UnvalidatedUpdateTicked += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Specialized.UnvalidatedUpdateTicked -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IUnvalidatedUpdateTickedHandler handler, object sender, UnvalidatedUpdateTickedEventArgs eventArgs)
			{
				handler.OnUnvalidatedUpdateTicked(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="ISpecializedEvents.UnvalidatedUpdateTicking" />.</summary>
        internal class UnvalidatedUpdateTickingEventManager : EventManager<IUnvalidatedUpdateTickingHandler, UnvalidatedUpdateTickingEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="UnvalidatedUpdateTickingEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public UnvalidatedUpdateTickingEventManager(IModHelper helper, ISimpleFactory<IUnvalidatedUpdateTickingHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.Specialized.UnvalidatedUpdateTicking += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.Specialized.UnvalidatedUpdateTicking -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IUnvalidatedUpdateTickingHandler handler, object sender, UnvalidatedUpdateTickingEventArgs eventArgs)
			{
				handler.OnUnvalidatedUpdateTicking(sender, eventArgs);
			}
        }
    }

    namespace World
    {

        /// <summary>Handles events from <see cref="IWorldEvents.BuildingListChanged" />.</summary>
        internal class BuildingListChangedEventManager : EventManager<IBuildingListChangedHandler, BuildingListChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="BuildingListChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public BuildingListChangedEventManager(IModHelper helper, ISimpleFactory<IBuildingListChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.World.BuildingListChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.World.BuildingListChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IBuildingListChangedHandler handler, object sender, BuildingListChangedEventArgs eventArgs)
			{
				handler.OnBuildingListChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IWorldEvents.DebrisListChanged" />.</summary>
        internal class DebrisListChangedEventManager : EventManager<IDebrisListChangedHandler, DebrisListChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="DebrisListChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public DebrisListChangedEventManager(IModHelper helper, ISimpleFactory<IDebrisListChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.World.DebrisListChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.World.DebrisListChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IDebrisListChangedHandler handler, object sender, DebrisListChangedEventArgs eventArgs)
			{
				handler.OnDebrisListChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IWorldEvents.LargeTerrainFeatureListChanged" />.</summary>
        internal class LargeTerrainFeatureListChangedEventManager : EventManager<ILargeTerrainFeatureListChangedHandler, LargeTerrainFeatureListChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="LargeTerrainFeatureListChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public LargeTerrainFeatureListChangedEventManager(IModHelper helper, ISimpleFactory<ILargeTerrainFeatureListChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.World.LargeTerrainFeatureListChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.World.LargeTerrainFeatureListChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ILargeTerrainFeatureListChangedHandler handler, object sender, LargeTerrainFeatureListChangedEventArgs eventArgs)
			{
				handler.OnLargeTerrainFeatureListChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IWorldEvents.LocationListChanged" />.</summary>
        internal class LocationListChangedEventManager : EventManager<ILocationListChangedHandler, LocationListChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="LocationListChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public LocationListChangedEventManager(IModHelper helper, ISimpleFactory<ILocationListChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.World.LocationListChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.World.LocationListChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ILocationListChangedHandler handler, object sender, LocationListChangedEventArgs eventArgs)
			{
				handler.OnLocationListChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IWorldEvents.NpcListChanged" />.</summary>
        internal class NpcListChangedEventManager : EventManager<INpcListChangedHandler, NpcListChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="NpcListChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public NpcListChangedEventManager(IModHelper helper, ISimpleFactory<INpcListChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.World.NpcListChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.World.NpcListChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(INpcListChangedHandler handler, object sender, NpcListChangedEventArgs eventArgs)
			{
				handler.OnNpcListChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IWorldEvents.ObjectListChanged" />.</summary>
        internal class ObjectListChangedEventManager : EventManager<IObjectListChangedHandler, ObjectListChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="ObjectListChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public ObjectListChangedEventManager(IModHelper helper, ISimpleFactory<IObjectListChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.World.ObjectListChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.World.ObjectListChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(IObjectListChangedHandler handler, object sender, ObjectListChangedEventArgs eventArgs)
			{
				handler.OnObjectListChanged(sender, eventArgs);
			}
        }

        /// <summary>Handles events from <see cref="IWorldEvents.TerrainFeatureListChanged" />.</summary>
        internal class TerrainFeatureListChangedEventManager : EventManager<ITerrainFeatureListChangedHandler, TerrainFeatureListChangedEventArgs>
        {
			private readonly IModHelper helper;

			/// <summary>
			/// Initializes a new instance of the <see cref="TerrainFeatureListChangedEventManager"/> class.
			/// </summary>
			/// <param name="helper"></param>
			/// <param name="getHandlers"></param>
			public TerrainFeatureListChangedEventManager(IModHelper helper, ISimpleFactory<ITerrainFeatureListChangedHandler> handlerFactory)
				: base(handlerFactory)
			{
				this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
			}

			/// <inheritdoc/>
			public override void StartListening()
			{
                this.helper.Events.World.TerrainFeatureListChanged += this.HandleEvent;
			}

			/// <inheritdoc/>
			public override void StopListening()
			{
                this.helper.Events.World.TerrainFeatureListChanged -= this.HandleEvent;
			}

			/// <inheritdoc/>
			protected override void NotifyHandler(ITerrainFeatureListChangedHandler handler, object sender, TerrainFeatureListChangedEventArgs eventArgs)
			{
				handler.OnTerrainFeatureListChanged(sender, eventArgs);
			}
        }
    }
}
#pragma warning restore SA1505
#pragma warning restore SA1649
#pragma warning restore SA1403