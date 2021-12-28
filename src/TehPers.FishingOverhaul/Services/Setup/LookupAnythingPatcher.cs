using HarmonyLib;
using Ninject;
using Ninject.Activation;
using StardewModdingAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using TehPers.Core.Api.Setup;

namespace TehPers.FishingOverhaul.Services.Setup
{
    [SuppressMessage(
        "ReSharper",
        "InconsistentNaming",
        Justification = "Harmony patches have a specific naming convention."
    )]
    internal partial class LookupAnythingPatcher : Patcher, ISetup
    {
        private static readonly Lazy<Type> customFieldInterface = new(
            () => AccessTools.TypeByName(
                "Pathoschild.Stardew.LookupAnything.Framework.Fields.ICustomField"
            )
        );

        private static readonly Lazy<Func<IEnumerable, object, IEnumerable>>
            enumerableAppendCustomField = new(
                () =>
                {
                    // Enumerable.Append<ICustomField>(...)
                    var enumerableAppend = AccessTools.Method(
                        typeof(Enumerable),
                        nameof(Enumerable.Append),
                        generics: new[] { LookupAnythingPatcher.customFieldInterface.Value }
                    );

                    // IEnumerable<ICustomField>
                    var enumerableCustomFields =
                        typeof(IEnumerable<>).MakeGenericType(
                            LookupAnythingPatcher.customFieldInterface.Value
                        );

                    var dataFieldsParam = Expression.Parameter(typeof(IEnumerable), "dataFields");
                    var customFieldParam = Expression.Parameter(typeof(object), "customField");
                    var dataFields = Expression.Convert(dataFieldsParam, enumerableCustomFields);
                    var customField = Expression.Convert(
                        customFieldParam,
                        LookupAnythingPatcher.customFieldInterface.Value
                    );
                    var appended = Expression.Call(null, enumerableAppend, dataFields, customField);
                    var result = Expression.Convert(appended, typeof(IEnumerable));

                    return Expression.Lambda<Func<IEnumerable, object, IEnumerable>>(
                            result,
                            dataFieldsParam,
                            customFieldParam
                        )
                        .Compile();
                }
            );

        private static LookupAnythingPatcher? Instance { get; set; }

        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly IManifest manifest;
        private readonly FishingApi fishingApi;

        private readonly Type tfoFieldType;

        private LookupAnythingPatcher(
            IModHelper helper,
            IMonitor monitor,
            IManifest manifest,
            Harmony harmony,
            FishingApi fishingApi
        )
            : base(harmony)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.manifest = manifest;
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));

            this.tfoFieldType = this.CreateTfoField();
        }

        public static LookupAnythingPatcher Create(IContext context)
        {
            LookupAnythingPatcher.Instance ??= new(
                context.Kernel.Get<IModHelper>(),
                context.Kernel.Get<IMonitor>(),
                context.Kernel.Get<IManifest>(),
                context.Kernel.Get<Harmony>(),
                context.Kernel.Get<FishingApi>()
            );
            return LookupAnythingPatcher.Instance;
        }

        public void Setup()
        {
            var itemSubjectType = AccessTools.TypeByName(
                "Pathoschild.Stardew.LookupAnything.Framework.Lookups.Items.ItemSubject"
            );

            this.Patch(
                AccessTools.Method(itemSubjectType, "GetData"),
                postfix: new(
                    AccessTools.Method(
                        typeof(LookupAnythingPatcher),
                        nameof(this.ItemSubject_GetData_Postfix)
                    )
                )
            );
        }

        private static void ItemSubject_GetData_Postfix(ref object __result)
        {
            if (LookupAnythingPatcher.Instance is not { } patcher)
            {
                return;
            }

            if (__result is not IEnumerable result)
            {
                patcher.monitor.Log("Item subject did not return an IEnumerable.", LogLevel.Warn);
                return;
            }

            var tfoField = Activator.CreateInstance(patcher.tfoFieldType)!;
            __result = LookupAnythingPatcher.enumerableAppendCustomField.Value(result, tfoField);
        }
    }
}