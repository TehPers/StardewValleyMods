using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Messages;

namespace TehPers.FishingOverhaul.Setup
{
    internal sealed class FishingMessageHandler : ISetup, IDisposable
    {
        private readonly IManifest manifest;
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly INamespaceRegistry namespaceRegistry;

        public FishingMessageHandler(
            IManifest manifest,
            IModHelper helper,
            IMonitor monitor,
            INamespaceRegistry namespaceRegistry
        )
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.namespaceRegistry = namespaceRegistry ?? throw new ArgumentNullException(nameof(namespaceRegistry));
        }

        public void Setup()
        {
            this.helper.Events.Multiplayer.ModMessageReceived += this.HandleMessage;
        }

        public void Dispose()
        {
            this.helper.Events.Multiplayer.ModMessageReceived -= this.HandleMessage;
        }

        private void HandleMessage(object? sender, ModMessageReceivedEventArgs e)
        {
            // Only handle this mod's messages
            if (e.FromModID != this.manifest.UniqueID)
            {
                return;
            }

            this.monitor.Log($"Handling multiplayer event for '{e.Type}'");
            switch (e.Type)
            {
                case FishingMessageTypes.TrashCaught:
                {
                    if (e.ReadAs<TrashCaughtMessage>() is not var (userId, itemKey))
                    {
                        this.monitor.Log("Error reading message.", LogLevel.Warn);
                        return;
                    }

                    if (Game1.getFarmer(userId) is not { } user)
                    {
                        this.monitor.Log($"Unknown farmer {userId}.", LogLevel.Warn);
                        return;
                    }

                    if (user.CurrentTool is not FishingRod rod)
                    {
                        this.monitor.Log($"Farmer {userId} is not holding a fishing rod.", LogLevel.Warn);
                        return;
                    }

                    this.monitor.Log($"Farmer {userId} caught trash: {itemKey}");
                    // TODO: this.rodOverrider.PullItemFromWater(user, rod, itemKey);
                    break;
                }
                case FishingMessageTypes.SpecialCaught:
                {
                    if (e.ReadAs<SpecialCaughtMessage>() is not var (userId, itemKey))
                    {
                        this.monitor.Log("Error reading message.", LogLevel.Warn);
                        return;
                    }

                    if (Game1.getFarmer(userId) is not { } user)
                    {
                        this.monitor.Log($"Unknown farmer {userId}.", LogLevel.Warn);
                        return;
                    }

                    if (user.CurrentTool is not FishingRod rod)
                    {
                        this.monitor.Log($"Farmer {userId} is not holding a fishing rod.", LogLevel.Warn);
                        return;
                    }

                    this.monitor.Log($"Farmer {userId} caught special item: {itemKey}");
                    // TODO: this.rodOverrider.PullItemFromWater(user, rod, itemKey);
                    break;
                }
                case FishingMessageTypes.StartFishing:
                {
                    if (e.ReadAs<StartFishingMessage>() is not var (userId, fishKey))
                    {
                        this.monitor.Log("Error reading message.", LogLevel.Warn);
                        return;
                    }

                    if (Game1.getFarmer(userId) is not { } user)
                    {
                        this.monitor.Log($"Unknown farmer {userId}.", LogLevel.Warn);
                        return;
                    }

                    this.monitor.Log($"Farmer {userId} hit fish: {fishKey}");
                    // TODO

                    break;
                }
                default:
                {
                    this.monitor.Log($"Unknown multiplayer message type: '{e.Type}'", LogLevel.Warn);
                    break;
                }
            }
        }
    }
}