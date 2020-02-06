using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Json;

namespace TehPers.Core.Modules
{
    public class JsonConvertersModule : ModModule
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;

        public JsonConvertersModule(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public override void Load()
        {
            // SMAPI's default converters
            foreach (var converter in this.GetSmapiConverters())
            {
                this.GlobalProxyRoot.Bind<JsonConverter>().ToConstant(converter).InSingletonScope();
            }

            this.GlobalProxyRoot.Bind<JsonConverter>().ToConstant(new NetConverter()).InSingletonScope();
            this.GlobalProxyRoot.Bind<JsonConverter>().ToConstant(new DescriptiveJsonConverter()).InSingletonScope();
            this.GlobalProxyRoot.Bind<JsonConverter>().ToConstant(new NamespacedIdJsonConverter()).InSingletonScope();
        }

        private IEnumerable<JsonConverter> GetSmapiConverters()
        {
            var smapiJsonHelper = this.helper.Data.GetType().GetField("JsonHelper", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this.helper.Data);
            var smapiJsonSettings = smapiJsonHelper?.GetType().GetProperty("JsonSettings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(smapiJsonHelper);
            if (smapiJsonSettings is JsonSerializerSettings { Converters: { } smapiConverters })
            {
                // Add all the converters SMAPI uses to this API's serializer settings
                foreach (var converter in smapiConverters)
                {
                    yield return converter;
                }
            }
            else
            {
                this.monitor.Log("Unable to get SMAPI's JSON converters. Some config settings might be confusing!", LogLevel.Error);
            }
        }
    }
}