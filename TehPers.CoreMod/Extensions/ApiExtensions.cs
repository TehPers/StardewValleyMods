using System;
using System.Collections.Generic;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Json;
using TehPers.CoreMod.Items;
using TehPers.CoreMod.Json;

namespace TehPers.CoreMod.Extensions {
    internal static class ApiExtensions {
        private static readonly ApiManager<IJsonApi> _jsonApiManager = new ApiManager<IJsonApi>(coreApi => new JsonApi(new ApiHelper(coreApi, "Json")));
        private static readonly ApiManager<IItemApi> _itemApiManager = new ApiManager<IItemApi>(coreApi => new ItemApi(new ApiHelper(coreApi, "Items")));

        /// <summary>Gets or creates a JSON API which can be used to create and read commented JSON files.</summary>
        /// <param name="coreApi">The core API to use for the JSON API.</param>
        /// <returns>The JSON API.</returns>
        public static IJsonApi GetJsonApi(this ICoreApi coreApi) {
            return ApiExtensions._jsonApiManager.GetOrCreate(coreApi);
        }

        /// <summary>Gets or creates an item API which can be used to create new items in the game.</summary>
        /// <param name="coreApi">The core API to use for the item API.</param>
        /// <returns>The item API.</returns>
        public static IItemApi GetItemApi(this ICoreApi coreApi) {
            return ApiExtensions._itemApiManager.GetOrCreate(coreApi);
        }

        private class ApiManager<T> {
            private readonly Func<ICoreApi, T> _apiFactory;
            private readonly Dictionary<ICoreApi, T> _generatedApis = new Dictionary<ICoreApi, T>();

            public ApiManager(Func<ICoreApi, T> apiFactory) {
                this._apiFactory = apiFactory;
            }

            public T GetOrCreate(ICoreApi coreApi) {
                if (this._generatedApis.TryGetValue(coreApi, out T api)) {
                    return api;
                }

                api = this._apiFactory(coreApi);
                this._generatedApis.Add(coreApi, api);
                return api;
            }
        }
    }
}
