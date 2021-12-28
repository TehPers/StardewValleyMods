using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace TehPers.SpriteMain.Patches
{
    internal abstract class Patcher : IDisposable
    {
        private readonly List<(MethodBase target, MethodInfo patch)> patches = new();
        protected Harmony Harmony { get; }

        protected Patcher(Harmony harmony)
        {
            this.Harmony = harmony;
        }

        protected void Patch(
            MethodBase target,
            HarmonyMethod? prefix = null,
            HarmonyMethod? postfix = null,
            HarmonyMethod? transpiler = null,
            HarmonyMethod? finalizer = null
        )
        {
            var patch = this.Harmony.Patch(target, prefix, postfix, transpiler, finalizer);
            this.patches.Add((target, patch));
        }

        public virtual void Dispose()
        {
            foreach (var (target, patch) in this.patches)
            {
                this.Harmony.Unpatch(target, patch);
            }
        }
    }
}