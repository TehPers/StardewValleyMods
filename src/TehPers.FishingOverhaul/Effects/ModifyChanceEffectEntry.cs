﻿using Ninject;
using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Effects;
using TehPers.FishingOverhaul.Services.Setup;

namespace TehPers.FishingOverhaul.Effects
{
    /// <summary>
    /// Modifies the chance of catching a fish when fishing.
    /// </summary>
    /// <param name="Type">The type of chance to modify.</param>
    /// <param name="Expression">An expression representing the operation. <c>x</c> represents the chance.</param>
    internal record ModifyChanceEffectEntry(
        ModifyChanceType Type,
        string Expression
    ) : FishingEffectEntry
    {
        public override IFishingEffect CreateEffect(IGlobalKernel kernel)
        {
            var manager = kernel.Get<ModifyChanceEffectManager>();
            return manager.CreateEffect(this);
        }
    }
}
