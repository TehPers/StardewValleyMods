using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Newtonsoft.Json;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Multiplayer;
using TehPers.Core.DependencyInjection.Lifecycle;
using TehPers.Core.Items;
using TehPers.Core.Json;
using TehPers.Core.Modules;

namespace TehPers.Core
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Type is instantiated by SMAPI")]
    public class ModEntry : Mod, IServiceDrivenMod
    {
        private IModKernelFactory modKernelFactory;
        private LifecycleService lifecycleService;

        public override void Entry(IModHelper helper)
        {
            this.Monitor.Log("Creating core API factory", LogLevel.Info);
            this.modKernelFactory = new ModKernelFactory();

            this.Register();
        }

        public void GameLoaded(IModKernel modKernel)
        {
            this.lifecycleService = modKernel.Get<LifecycleService>();
            this.lifecycleService.StartAll();
        }

        [Obsolete]
        public class Blah
        {
            public static IMonitor Monitor;

            public Blah()
            {
                Blah.Monitor.Log("a", LogLevel.Alert);
            }
        }

        public void RegisterServices(IModKernel modKernel)
        {
            this.Monitor.Log("Registering mod services", LogLevel.Info);
            modKernel.Bind<LifecycleService>().ToSelf().InSingletonScope();

            this.Monitor.Log("Registering event managers", LogLevel.Info);
            modKernel.BindManagedSmapiEvents();

            this.Monitor.Log("Registering global services", LogLevel.Info);
            modKernel.ParentFactory.LoadIntoModKernels<CoreApiModModule>();
            modKernel.Bind<EventChannelFactory>().ToSelf().InSingletonScope();
            modKernel.ExposeService<EventChannelFactory>();
            modKernel.Load(new ItemProvidersModule());

            Blah.Monitor = this.Monitor;
            modKernel.Bind<Blah>().ToSelf().InSingletonScope();
            modKernel.GlobalKernel.Bind<Blah>().ToSelf().InSingletonScope();
            _ = modKernel.Get<Blah>(new GlobalParameter());
            _ = modKernel.GlobalKernel.Get<Blah>();

            // SMAPI's default converters
            foreach (var converter in this.GetSmapiConverters())
            {
                modKernel.GlobalRoot.Bind<JsonConverter>().ToConstant(converter).InSingletonScope();
            }

            modKernel.GlobalRoot.Bind<JsonConverter>().ToConstant(new NetConverter()).InSingletonScope();
            modKernel.GlobalRoot.Bind<JsonConverter>().ToConstant(new DescriptiveJsonConverter()).InSingletonScope();
            modKernel.GlobalRoot.Bind<JsonConverter>().ToConstant(new NamespacedIdJsonConverter()).InSingletonScope();
        }

        private IEnumerable<JsonConverter> GetSmapiConverters()
        {
            var smapiJsonHelper = this.Helper.Data.GetType().GetField("JsonHelper", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this.Helper.Data);
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
                this.Monitor.Log("Unable to get SMAPI's JSON converters. Some config settings might be confusing!", LogLevel.Error);
            }
        }

        public override object GetApi()
        {
            return this.modKernelFactory;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.modKernelFactory?.Dispose();
                this.lifecycleService?.StopAll();
            }

            base.Dispose(disposing);
        }
    }
}