using StardewModdingAPI;
using TehPers.Core.Api.Json;

namespace TehPers.Core.Items
{
    public class SObjectIndexRegistry : IndexRegistry
    {
        public SObjectIndexRegistry(IJsonProvider json, IModHelper helper)
            : base(json, helper, "objects", 1000000)
        {
        }
    }
}