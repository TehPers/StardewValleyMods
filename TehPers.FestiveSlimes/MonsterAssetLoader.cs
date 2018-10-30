using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TehPers.FestiveSlimes {
    public class MonsterAssetLoader : IAssetLoader {
        private readonly IMod _owner;
        public bool Replaced { get; private set; }

        public MonsterAssetLoader(IMod owner) {
            this._owner = owner;
            this.Replaced = this.ShouldReplace();
        }

        public void CheckForChanges() {
            bool shouldReplace = this.ShouldReplace();
            if (shouldReplace != this.Replaced) {
                this.Replaced = shouldReplace;
                this._owner.Helper.Content.InvalidateCache("Characters/Monsters/Green Slime");
                this._owner.Helper.Content.InvalidateCache("Characters/Monsters/Big Slime");
            }
        }

        private bool ShouldReplace() {
            return SDate.Now().Season.Equals("fall", StringComparison.OrdinalIgnoreCase);
        }

        public bool CanLoad<T>(IAssetInfo asset) {
            return this.Replaced && (
                       asset.AssetNameEquals("Characters/Monsters/Green Slime")
                       || asset.AssetNameEquals("Characters/Monsters/Big Slime")
                   );
        }

        public T Load<T>(IAssetInfo asset) {
            if (asset.AssetNameEquals("Characters/Monsters/Green Slime")) {
                return ModFestiveSlimes.Instance.Helper.Content.Load<T>("assets/Green Slime.png");
            } else if (asset.AssetNameEquals("Characters/Monsters/Big Slime")) {
                return ModFestiveSlimes.Instance.Helper.Content.Load<T>("assets/Big Slime.png"); ;
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
    }
}