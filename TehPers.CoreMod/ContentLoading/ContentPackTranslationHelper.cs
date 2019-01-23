using System.Collections.Generic;
using System.Linq;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.ContentLoading;

namespace TehPers.CoreMod.ContentLoading {
    internal class ContentPackTranslationHelper : ICoreTranslationHelper {
        private readonly Dictionary<string, string> _translations;

        public ContentPackTranslationHelper(ICoreApi api, IContentSource contentSource, string locale, Dictionary<string, string> translations) {
            this._translations = translations;
            api.Json.ReadJson<Dictionary<string, string>>($"i18n/{(string.IsNullOrEmpty(locale) ? "default" : locale)}.json", contentSource, settings => { });
        }

        public ICoreTranslation Get(string key) {
            return this._translations.TryGetValue(key, out string value) ? new CoreTranslation(key, value) : new CoreTranslation(key);
        }

        public IEnumerable<ICoreTranslation> GetAll() {
            return this._translations.Select(kv => new CoreTranslation(kv.Key, kv.Value));
        }
    }
}