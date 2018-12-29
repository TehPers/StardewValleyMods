using System;
using StardewModdingAPI;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Drawing;
using TehPers.CoreMod.Items;

namespace TehPers.CoreMod {
    internal class CoreApi : ICoreApi {
        private readonly Lazy<IDrawingApi> _drawing;
        private readonly Lazy<IItemApi> _items;

        public IMod Owner { get; }
        public IDrawingApi Drawing => this._drawing.Value;
        public IItemApi Items => this._items.Value;

        public CoreApi(IMod owner) {
            this.Owner = owner;
            this._drawing = new Lazy<IDrawingApi>(() => new DrawingApi(this));
            this._items = new Lazy<IItemApi>(() => new ItemApi(this));
        }
    }
}