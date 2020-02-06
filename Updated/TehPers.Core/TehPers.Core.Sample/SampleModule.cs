using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Sample
{
    internal class SampleModule : ModModule
    {
        public override void Load()
        {
            // Bind a config
            this.BindConfiguration<ModConfig>("config.json");
        }
    }
}
