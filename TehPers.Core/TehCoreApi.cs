using System;
using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Helpers;

namespace TehPers.Core {
    public class TehCoreApi {
        #region Static
        private static readonly Dictionary<IMod, TehCoreApi> _apis = new Dictionary<IMod, TehCoreApi>();

        public static TehCoreApi Create(IMod owner) {
            if (!TehCoreApi._apis.TryGetValue(owner, out TehCoreApi api)) {
                api = new TehCoreApi(owner);
                TehCoreApi._apis[owner] = api;
            }

            return api;
        }
        #endregion

        public IMod Owner { get; }
        public JsonHelper JsonHelper { get; }
        public Action<string, LogLevel> Log { get; set; }

        private TehCoreApi(IMod owner) {
            this.Owner = owner;
            this.JsonHelper = new JsonHelper(this);

            this.Log = (message, level) => owner.Monitor.Log($"[TehPers.Core] {message}", level);
        }
    }
}
