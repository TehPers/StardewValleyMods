using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Static.Enums;
using TehPers.CoreMod.Api.Static.Extensions;
using TehPers.Logistics.Items;
using TehPers.Logistics.Machines;

namespace TehPers.Logistics {
    public class StoneConverterMachine : ModCraftable, IMachine {
        public StoneConverterMachine(IMod owner, string rawName, TextureInformation textureInfo) : base(owner, rawName, 500, textureInfo) { }

        public IEnumerable<IEnumerable<ItemRequest>> Accept(IMachineInformation machineInfo, IEnumerable<ItemRequest> items, Vector2 inputTile, out MachineAction doAccept) {
            State state = machineInfo.GetState<State>();

            // Check if already processing
            if (state.Processing) {
                doAccept = default;
                return null;
            }

            // Get all available stone
            IEnumerable<ItemRequest> stone = items.Where(i => i.Item.ParentSheetIndex == Objects.Stone);

            // Create requests for up to 100 stone
            List<ItemRequest> requests = new List<ItemRequest>();
            int remaining = 100;
            foreach (ItemRequest request in stone) {
                // Create a new request for stone
                if (request.Quantity <= remaining) {
                    // Requests are structs, so it's fine to just pass it around like this
                    requests.Add(request);
                    remaining -= request.Quantity;
                } else {
                    requests.Add(new ItemRequest(request.Item, remaining));
                    remaining = 0;
                }

                // Check if reached 100 stone
                if (remaining == 0) {
                    // If the machine actually accepts something, make sure to update the state of the machine
                    doAccept = payload => {
                        state.Processing = true;
                        state.StartTime = SDateTime.Now;
                    };

                    // Return the requested stone as a single payload
                    return requests.AsEnumerable().Yield();
                }
            }

            // There isn't 100 stone available, so return null
            doAccept = default;
            return null;
        }

        public IEnumerable<IEnumerable<ItemRequest>> Eject(IMachineInformation machineInfo, IEnumerable<ItemRequest> items, Vector2 outputTile, out MachineAction doEject) {
            State state = machineInfo.GetState<State>();

            // Check if not done
            if (!(state.Processing && (SDateTime.Now - state.StartTime).TotalMinutes >= 60)) {
                doEject = null;
                return null;
            }

            // If the machine actually ejects something, make sure to update the state of the machine
            doEject = payload => state.Processing = false;

            // Return a new diamond payload
            return new ItemRequest(new Object(Vector2.Zero, Objects.Diamond, 1)).Yield().Yield();
        }

        public void UpdateTick(IMachineInformation machineInfo) { }

        public IEnumerable<ItemRequest> InsertItem(IMachineInformation machineInfo, Item heldItem, IEnumerable<Item> inventory, Farmer source, out MachineAction doInsert) {
            State state = machineInfo.GetState<State>();

            // Check if already processing
            if (state.Processing) {
                doInsert = default;
                return null;
            }

            // Check if enough stone is being held
            if (heldItem.ParentSheetIndex == Objects.Stone && heldItem.Stack >= 100) {
                // If stone is actually inserted into the machine, make sure to update the state
                doInsert = payload => {
                    state.Processing = true;
                    state.StartTime = SDateTime.Now;
                };

                // Request 100 stone
                return new ItemRequest(heldItem, 100).Yield();
            }

            // Don't request anything
            doInsert = default;
            return null;
        }

        public IEnumerable<ItemRequest> RemoveItem(IMachineInformation machineInfo, Farmer source, out MachineAction doRemove) {
            State state = machineInfo.GetState<State>();

            // Check if not done
            if (!(state.Processing && (SDateTime.Now - state.StartTime).TotalMinutes >= 60)) {
                doRemove = default;
                return null;
            }

            // If the machine actually ejects something, make sure to update the state of the machine
            doRemove = payload => state.Processing = false;

            // Return a new diamond payload
            return new ItemRequest(new Object(Vector2.Zero, Objects.Diamond, 1)).Yield();
        }

        public class State {
            public bool Processing { get; set; } = false;
            public SDateTime StartTime { get; set; }
        }
    }
}