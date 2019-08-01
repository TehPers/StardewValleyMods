using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.FishingFramework.Api
{
    public interface IFishingApi
    {
        IList<IFishData> RegisteredFish { get; }
        IList<ITrashData> RegisteredTrash { get; }
        IList<ITreasureData> RegisteredTreasure { get; }
    }

    public interface IFishData : IAvailabilityData
    {

    }

    public interface IFishBehavior
    {
        float BaseDifficulty { get; }
        float MinSize { get; }
        float MaxSize { get; }
        IFishMotionController MotionController { get; }
    }

    public interface IFishMotionController
    {

    }

    public interface ITrashData : IAvailabilityData
    {

    }

    public interface ITreasureData : IAvailabilityData
    {

    }

    public interface IAvailabilityData
    {
        bool IsAvailable();
    }
}
