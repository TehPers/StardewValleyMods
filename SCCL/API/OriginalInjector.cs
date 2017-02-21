using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.API {
    internal class OriginalInjector : ContentInjector {
        public override bool Enabled {
            get {
                return true;
            }
            set { }
        }
        
        public OriginalInjector() : base("[ORIGINAL]") { }

        public void registerOriginal(ref object original, string assetName) {

        }
    }
}
