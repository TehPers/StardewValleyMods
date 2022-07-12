using System;

namespace TehPers.Core.Api.Gui.States
{
    /// <summary>
    /// The state of a scrollbar.
    /// </summary>
    public class ScrollbarState
    {
        private int value;

        /// <summary>
        /// Gets or sets the inclusive minimum value of this scrollbar.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Gets or sets the inclusive maximum value of this scrollbar.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the current value this scrollbar holds.
        /// </summary>
        public int Value
        {
            get => Math.Clamp(this.value, this.MinValue, this.MaxValue);
            set => this.value = value;
        }

        /// <summary>
        /// Gets the percentage the current value is between the minimum and maximum value.
        /// </summary>
        public float Percentage =>
            (float)(this.value - this.MinValue) / (this.MaxValue - this.MinValue);
    }
}
