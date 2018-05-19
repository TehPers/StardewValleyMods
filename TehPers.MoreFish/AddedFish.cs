using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Enums;

namespace TehPers.MoreFish {
    public class AddedFish {
        public IFishingApi Api { get; }
        public int ParentSheetIndex { get; }
        public FishTraits FishTraits { get; }
        public string AssetName { get; }
        public string AssetKey { get; }

        public AddedFish(IFishingApi api, int parentSheetIndex, FishTraits traits, string assetName) : this(api, parentSheetIndex, traits, assetName, null) { }

        public AddedFish(IFishingApi api, int parentSheetIndex, FishTraits traits, string assetName, string assetKey) {
            if (AddedFish.Fish.Any(f => f.ParentSheetIndex == parentSheetIndex))
                throw new ArgumentException("ID already taken", nameof(parentSheetIndex));

            this.Api = api;
            this.ParentSheetIndex = parentSheetIndex;
            this.FishTraits = traits;
            this.AssetName = assetName;
            this.AssetKey = assetKey ?? traits.Name.ToLower();
            AddedFish.Fish.Add(this);
            api.SetFishTraits(parentSheetIndex, traits);
            api.SetFishName(parentSheetIndex, traits.Name);
        }

        public void SetCatchData(string location, FishData data) {
            this.Api.SetFishData(location, this.ParentSheetIndex, data);
        }

        #region Static
        internal static void LoadFish(IFishingApi api) {
            // Add the fish
            AddedFish.Seahorse = new AddedFish(api, 900, new FishTraits("Seahorse", 100, 1, 5, FishMotionType.DART, 600, 10), "seahorse.png");
            AddedFish.Seahorse2 = new AddedFish(api, 901, new FishTraits("Seahorse", 100, 3, 10, FishMotionType.DART, 600, 10), "seahorse2.png");

            // Set their catch data
            AddedFish.Seahorse.SetCatchData("Beach", new FishData(0.1F, new[] { new TimeInterval(600, 2600) }));
            AddedFish.Seahorse2.SetCatchData("Beach", new FishData(0.1F, new[] { new TimeInterval(600, 2600) }));

            // Hide hidden fish
            const bool hide = false;
            if (hide) {
                api.HideFish(AddedFish.Seahorse.ParentSheetIndex);
                api.HideFish(AddedFish.Seahorse2.ParentSheetIndex);
            }

            // Create the asset editor
            ModFish.Instance.Helper.Content.AssetEditors.Add(new AssetEditor());
        }

        // Fish data
        public static List<AddedFish> Fish { get; } = new List<AddedFish>();

        // Fish
        public static AddedFish Seahorse { get; private set; }
        public static AddedFish Seahorse2 { get; private set; }
        #endregion
    }
}
