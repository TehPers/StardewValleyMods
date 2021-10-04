using System.Collections.Generic;

namespace TehPers.FishingOverhaul.Api.Content
{
    public interface IFishingContentSource
    {
        IEnumerable<FishingContent> Reload();
    }
}