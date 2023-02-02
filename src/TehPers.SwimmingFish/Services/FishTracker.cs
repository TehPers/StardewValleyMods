using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api;
using TehPers.SwimmingFish.Models;

namespace TehPers.SwimmingFish.Services
{
    internal sealed class FishTracker : ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly IFishingApi fishingApi;
        private readonly INamespaceRegistry registry;
        private readonly List<TrackedFish> fish = new();
        private GameLocation? currentLocation;

        public FishTracker(IModHelper helper, IFishingApi fishingApi, INamespaceRegistry registry)
        {
            this.helper = helper;
            this.fishingApi = fishingApi;
            this.registry = registry;
        }

        public void Setup()
        {
            this.helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            this.helper.Events.Player.Warped += this.Warped;
        }

        public void Dispose()
        {
            this.helper.Events.GameLoop.UpdateTicked -= this.UpdateTicked;
            this.helper.Events.Player.Warped -= this.Warped;
        }

        private void UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (this.currentLocation?.waterTiles is not { } waterTiles)
            {
                return;
            }

            // Update fish
            var mapTileWidth = waterTiles.waterTiles.GetLength(0);
            var mapTileHeight = waterTiles.waterTiles.GetLength(1);
            this.fish.RemoveAll(
                fish =>
                {
                    var tileLeft = (int)(fish.Position.X / 64.0f);
                    var tileRight = (int)(fish.Position.X / 64.0f + fish.Scale);
                    var tileTop = (int)(fish.Position.Y / 64.0f);
                    var tileBottom = (int)(fish.Position.Y / 64.0f + fish.Scale);

                    // Despawn out of bounds fish
                    if (tileLeft < 0
                        || tileRight >= mapTileWidth
                        || tileTop < 0
                        || tileBottom >= mapTileHeight)
                    {
                        return true;
                    }

                    // Despawn fish on land
                    if (!waterTiles[tileLeft, tileTop])
                    {
                        return true;
                    }

                    // Tick fish
                    if (fish.SpawnTicksRemaining > 0)
                    {
                        fish.SpawnTicksRemaining -= 1;
                    }
                    else if (fish.TicksRemaining > 0)
                    {
                        fish.TicksRemaining -= 1;
                    }
                    else if (fish.DespawnTicksRemaining > 0)
                    {
                        fish.DespawnTicksRemaining -= 1;
                    }
                    else
                    {
                        // Despawn expired fish
                        return true;
                    }

                    // Bounce fish
                    var fishWidth = 64f * fish.Scale;
                    var fishSize = new Vector2(fishWidth, fishWidth);
                    if (this.WillCollide(fish.Position, fish.Velocity, fishSize))
                    {
                        fish.Velocity = new(fish.Velocity.X * -1, fish.Velocity.Y);
                    }

                    // Update position
                    fish.Position += fish.Velocity;

                    return false;
                }
            );

            // Spawn new fish
            var maxFish = mapTileWidth * mapTileHeight / 10;
            if (this.fish.Count < maxFish)
            {
                var spawnableTiles = Enumerable.Range(0, mapTileWidth)
                    .SelectMany(x => Enumerable.Range(0, mapTileHeight).Select(y => (x, y)))
                    .Where(t => waterTiles[t.x, t.y])
                    .ToList();
                if (spawnableTiles.Count > 0)
                {
                    while (this.fish.Count < maxFish)
                    {
                        // Choose a random spawn tile
                        var (tileX, tileY) =
                            spawnableTiles[Game1.random.Next(spawnableTiles.Count)];

                        // Spawn the fish
                        if (this.GenerateFish(tileX, tileY) is not { } fish)
                        {
                            // Don't try to spawn anymore fish to avoid infinite loops
                            break;
                        }

                        this.fish.Add(fish);
                    }
                }
            }
        }

        private bool WillCollide(Vector2 position, Vector2 velocity, Vector2 size)
        {
            if (this.currentLocation?.waterTiles is not { } waterTiles)
            {
                return false;
            }

            var newPosition = position + velocity;
            var mapTileWidth = waterTiles.waterTiles.GetLength(0);

            // Check left collision
            if (velocity.X <= 0)
            {
                var newTileLeft = (int)(newPosition.X / 64.0f);
                var newTileTop = (int)(newPosition.Y / 64.0f);
                var newTileBottom = (int)((newPosition.Y + size.Y) / 64.0f);

                // Check map bounds
                if (newTileLeft < 0)
                {
                    return true;
                }

                // Check for land collision
                for (var tileY = newTileTop; tileY <= newTileBottom; tileY++)
                {
                    if (tileY < 0 || !waterTiles.waterTiles[newTileLeft, tileY].isWater)
                    {
                        return true;
                    }
                }
            }

            // Check right collision
            if (velocity.X >= 0)
            {
                var newTileRight = (int)((newPosition.X + size.X) / 64.0f);
                var newTileTop = (int)(newPosition.Y / 64.0f);
                var newTileBottom = (int)((newPosition.Y + size.Y) / 64.0f);

                // Check map bounds
                if (newTileRight >= mapTileWidth)
                {
                    return true;
                }

                // Check for land collision
                for (var tileY = newTileTop; tileY <= newTileBottom; tileY++)
                {
                    if (!waterTiles.waterTiles[newTileRight, tileY].isWater)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Warped(object? sender, WarpedEventArgs e)
        {
            // Update current location
            this.currentLocation = e.NewLocation;

            // Generate new fish
            this.fish.Clear();
        }

        private TrackedFish? GenerateFish(int x, int y)
        {
            // Select a catch
            var possibleCatch = this.fishingApi.GetPossibleCatch(
                this.fishingApi.CreateDefaultFishingInfo(Game1.player) with
                {
                    BobberPosition = new(x, y),
                }
            );
            var (itemKey, isFish) = possibleCatch switch
            {
                PossibleCatch.Fish(var entry) => (entry.FishKey, true),
                PossibleCatch.Trash(var entry) => (entry.ItemKey, false),
                _ => throw new InvalidOperationException(
                    $"Unknown possible catch type: {possibleCatch.GetType()}"
                ),
            };
            if (!this.registry.TryGetItemFactory(itemKey, out var factory))
            {
                return null;
            }

            // Create the fish
            // TODO: config options?
            var item = factory.Create();
            var scale = 2.0f * (float)Game1.random.NextDouble() + 2.0f; // TODO
            var position = new Vector2(x * 64.0f, y * 64.0f);
            var velocity = new Vector2(
                isFish
                    ? 1.0f * (float)Game1.random.NextDouble() - 0.5f
                    : 0.2f * (float)Game1.random.NextDouble() - 0.1f,
                0.0f
            );
            var lifetime = Game1.random.Next(30 * 60, 120 * 60);
            return new(itemKey, item, isFish, position, velocity, scale, 60, lifetime, 60);
        }

        public IEnumerable<TrackedFish> GetFish()
        {
            return this.fish;
        }
    }
}
