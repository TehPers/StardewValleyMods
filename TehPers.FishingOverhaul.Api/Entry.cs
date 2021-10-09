using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace TehPers.FishingOverhaul.Api
{
    public abstract class Entry<T>
        where T : AvailabilityInfo
    {
        [JsonRequired]
        [Description("The availability information.")]
        public T AvailabilityInfo { get; set; }

        protected Entry(T availabilityInfo)
        {
            this.AvailabilityInfo = availabilityInfo
                ?? throw new ArgumentNullException(nameof(availabilityInfo));
        }
    }
}