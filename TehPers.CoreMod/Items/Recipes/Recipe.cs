// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using StardewValley;
// using TehPers.CoreMod.Items.Inventory;
// using SObject = StardewValley.Object;
// 
// namespace TehPers.CoreMod.Items.Recipes {
//     internal class Recipe {
//         public bool TryCreate(IInventory inventory, out Item result) {
// 
//         }
//     }
// }
// 
// namespace TehPers.CoreMod.Items.Inventory {
//     internal class FarmerInventory : IInventory {
//         private readonly Farmer _who;
// 
//         public FarmerInventory(Farmer who) {
//             this._who = who;
//         }
// 
//         public IEnumerator<Item> GetEnumerator() {
//             return this._who.Items.GetEnumerator();
//         }
// 
//         IEnumerator IEnumerable.GetEnumerator() {
//             return ((IEnumerable) this).GetEnumerator();
//         }
// 
//         public Item Add(Item item) {
//             return this._who.addItemToInventory(item);
//         }
// 
//         public bool Remove(Item item) {
//             if (!this._who.Items.Contains(item)) {
//                 return false;
//             }
// 
//             this._who.removeItemFromInventory(item);
//             return true;
// 
//         }
// 
//         public bool TryRemove(SObject item, out int unavailable) {
//             throw new NotImplementedException();
//         }
//     }
// 
//     public interface IInventory : IEnumerable<Item> {
//         /// <summary>Adds as much of an item to this inventory as possible.</summary>
//         /// <param name="item">The item to add to this inventory.</param>
//         /// <returns>The remaining item.</returns>
//         Item Add(Item item);
// 
//         /// <summary>Removes an item from this inventory. Items are compared by reference.</summary>
//         /// <param name="item">The item to remove from this inventory.</param>
//         /// <returns>True if removed, false otherwise.</returns>
//         bool Remove(Item item);
// 
//         /// <summary>Tries to remove the given quantity of an <see cref="SObject"/> from this inventory. The quantity is determined by the <see cref="SObject.Stack"/> of the <paramref name="item"/>.</summary>
//         /// <param name="item">The item to remove.</param>
//         /// <param name="unavailable">If failed, the amount of the item that could not be removed.</param>
//         /// <returns></returns>
//         bool TryRemove(SObject item, out int unavailable);
//     }
// }
