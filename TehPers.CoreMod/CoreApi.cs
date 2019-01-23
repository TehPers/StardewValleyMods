using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Json;
using TehPers.CoreMod.ContentLoading;
using TehPers.CoreMod.Drawing;
using TehPers.CoreMod.Drawing.Sprites;
using TehPers.CoreMod.Items;
using TehPers.CoreMod.Json;

namespace TehPers.CoreMod {
    internal class CoreApi : ICoreApi {
        private readonly Lazy<IDrawingApi> _drawing;
        private readonly Lazy<IItemApi> _items;
        private readonly Lazy<IJsonApi> _json;

        public IMod Owner { get; }
        public IDrawingApi Drawing => this._drawing.Value;
        public IItemApi Items => this._items.Value;
        public IJsonApi Json => this._json.Value;

        public CoreApi(IMod owner, DynamicSpriteSheet customItemSpriteSheet) {
            this.Owner = owner;
            this._drawing = new Lazy<IDrawingApi>(() => new DrawingApi(new ApiHelper(this, "Drawing")));
            this._items = new Lazy<IItemApi>(() => new ItemApi(new ApiHelper(this, "Items"), customItemSpriteSheet));
            this._json = new Lazy<IJsonApi>(() => new JsonApi(new ApiHelper(this, "Json")));
        }

        public ICoreTranslation Get(string key) {
            return new CoreTranslation(this.Owner.Helper.Translation.Get(key));
        }

        public IEnumerable<ICoreTranslation> GetAll() {
            return this.Owner.Helper.Translation.GetTranslations().Select(t => new CoreTranslation(t));
        }
    }
}