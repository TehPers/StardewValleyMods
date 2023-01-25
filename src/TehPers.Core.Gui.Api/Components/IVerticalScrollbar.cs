using System;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A vertical scrollbar.
/// </summary>
public interface IVerticalScrollbar : IGuiComponent, IWithLayerDepth<IVerticalScrollbar>,
    IWithState<IVerticalScrollbar.IState, IVerticalScrollbar>
{
    /// <summary>
    /// The state of a scrollbar.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets or sets the current value this scrollbar holds.
        /// </summary>
        int Value { get; set; }

        /// <summary>
        /// Gets or sets the inclusive minimum value of this scrollbar.
        /// </summary>
        int MinValue { get; set; }

        /// <summary>
        /// Gets or sets the inclusive maximum value of this scrollbar.
        /// </summary>
        int MaxValue { get; set; }

        /// <summary>
        /// Gets the percentage the current value is between the minimum and maximum value.
        /// </summary>
        float Percentage =>
            (float)(this.Value - this.MinValue) / (this.MaxValue - this.MinValue);
    }

    /// <inheritdoc />
    public class State : IState
    {
        private int value;

        /// <inheritdoc />
        public int MinValue { get; set; }

        /// <inheritdoc />
        public int MaxValue { get; set; }

        /// <inheritdoc />
        public int Value
        {
            get => Math.Clamp(this.value, this.MinValue, this.MaxValue);
            set => this.value = value;
        }
    }
}
