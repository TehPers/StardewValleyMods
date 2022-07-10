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
        /// The inclusive minimum value of this scrollbar.
        /// </summary>
        public int MinimumValue { get; set; }

        /// <summary>
        /// The inclusive maximum value of this scrollbar.
        /// </summary>
        public int MaximumValue { get; set; }

        /// <summary>
        /// The current value this scrollbar holds.
        /// </summary>
        public int Value
        {
            get => Math.Clamp(this.value, this.MinimumValue, this.MaximumValue);
            set => this.value = value;
        }
    }
}
