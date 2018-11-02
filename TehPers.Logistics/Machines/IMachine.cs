using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.CoreMod.Api.Items;

namespace TehPers.Logistics.Machines {
    public interface IMachine : IModObject {
        /// <summary>Tries to accept an item into this machine.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        /// <param name="items">The items that are available.</param>
        /// <param name="inputTile">The tile to input from (for directional input).</param>
        /// <param name="doAccept">A callback which performs the action of accepting an item. Until this method is called, the state of the machine should not actually change. This may be called multiple times if multiple payloads are returned.</param>
        /// <returns>The item payloads that were accepted by this machine. Each payload is an <see cref="IEnumerable{T}"/> containing requests that should be sent together. The return value is an <see cref="IEnumerable{T}"/> of payloads. If nothing should be accepted, use <c>null</c> or <see cref="Enumerable.Empty{T}"/>.</returns>
        IEnumerable<IEnumerable<ItemRequest>> Accept(IMachineInformation machineInfo, IEnumerable<ItemRequest> items, Vector2 inputTile, out MachineAction doAccept);

        /// <summary>Tries to eject an item from this machine.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        /// <param name="items">The items that can be ejected.</param>
        /// <param name="outputTile">The tile to output to (for directional output).</param>
        /// <param name="doEject">A callback which performs the action of ejecting an item. Until this method is called, the state of the machine should not actually change. This may be called multiple times if multiple payloads are returned.</param>
        /// <returns>The item payloads that were ejected by this machine. Each payload is an <see cref="IEnumerable{T}"/> containing requests that should be sent together. The return value is an <see cref="IEnumerable{T}"/> of payloads. If nothing should be ejected, use <c>null</c> or <see cref="Enumerable.Empty{T}"/>.</returns>
        IEnumerable<IEnumerable<ItemRequest>> Eject(IMachineInformation machineInfo, IEnumerable<ItemRequest> items, Vector2 outputTile, out MachineAction doEject);

        /// <summary>Called every tick the machine is in the world.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        void UpdateTick(IMachineInformation machineInfo);

        /// <summary>Called whenever a farmer tries to place an item into this machine.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        /// <param name="heldItem">The item held by the farmer when activating the machine.</param>
        /// <param name="inventory">The items in the farmer's inventory, including the held item.</param>
        /// <param name="source">The farmer who activated this machine.</param>
        /// <param name="doInsert">A callback which performs the action of inserting an item. Until this method is called, the state of the machine should not actually change. This callback should not remove the inserted items from the farmer.</param>
        /// <returns>The item payload accepted by this machine. If nothing was accepted, use <c>null</c> or <see cref="Enumerable.Empty{T}"/>. This method should not remove the items from the farmer directly, but should return the accepted items as a payload instead.</returns>
        IEnumerable<ItemRequest> InsertItem(IMachineInformation machineInfo, Item heldItem, IEnumerable<Item> inventory, Farmer source, out MachineAction doInsert);

        /// <summary>Called whenever a farmer tries to remove an item from this machine.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        /// <param name="source">The farmer who activated this machine.</param>
        /// <param name="doRemove">A callback which performs the action of removing an item. Until this method is called, the state of the machine should not actually change. This callback should not add the removed items to the farmer.</param>
        /// <returns>The item payload removed from this machine. If nothing was removed, use <c>null</c> or <see cref="Enumerable.Empty{T}"/>. This method should not add the items to the farmer directly, but should return the removed items as a payload instead.</returns>
        IEnumerable<ItemRequest> RemoveItem(IMachineInformation machineInfo, Farmer source, out MachineAction doRemove);
    }

    public delegate void MachineAction(IEnumerable<ItemRequest> payload);
}