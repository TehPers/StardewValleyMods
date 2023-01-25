using System;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A dropdown choice selector.
/// </summary>
/// <typeparam name="T">The type of item the dropdown holds.</typeparam>
public interface IDropdown<T> : IGuiComponent, IWithState<IDropdown<T>.IState, IDropdown<T>>,
    IWithLayerDepth<IDropdown<T>>
{
    /// <summary>
    /// The state of a dropdown.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// The maximum number of items that should be visible at a time.
        /// </summary>
        int MaxVisibleItems { get; set; }

        /// <summary>
        /// The index of the top item that is currently visible.
        /// </summary>
        int TopVisibleIndex { get; set; }

        /// <summary>
        /// The index of the hovered item.
        /// </summary>
        int? HoveredIndex { get; set; }

        /// <summary>
        /// Gets or sets whether the dropdown is dropped.
        /// </summary>
        bool Dropped { get; set; }

        /// <summary>
        /// Gets or sets the items in the dropdown.
        /// </summary>
        List<(T Item, string Label)> Items { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        int SelectedIndex { get; set; }

        /// <summary>
        /// The selected item, if any.
        /// </summary>
        (T Item, string Label)? Selected =>
            this.Items.Any() ? this.Items[this.SelectedIndex] : null;
    }

    /// <inheritdoc />
    public class State : IState
    {
        private int index;
        private int maxVisibleItems = 4;
        private int topVisibleIndex;
        private int? hoveredIndex;

        /// <inheritdoc />
        public bool Dropped { get; set; }

        /// <inheritdoc />
        public List<(T Item, string Label)> Items { get; set; }

        /// <inheritdoc />
        public int SelectedIndex
        {
            get => this.Items.Any() ? Math.Clamp(this.index, 0, this.Items.Count - 1) : 0;
            set => this.index = value;
        }

        /// <inheritdoc />
        public int MaxVisibleItems
        {
            get => this.maxVisibleItems;
            set => this.maxVisibleItems = Math.Min(1, value);
        }

        /// <inheritdoc />
        public int TopVisibleIndex
        {
            get => this.Items.Count > this.MaxVisibleItems
                ? Math.Clamp(this.topVisibleIndex, 0, this.Items.Count - this.MaxVisibleItems)
                : 0;
            set => this.topVisibleIndex = value;
        }

        /// <inheritdoc />
        public int? HoveredIndex
        {
            get => this.hoveredIndex is { } hoveredIndex && this.Dropped && this.Items.Any()
                ? Math.Clamp(hoveredIndex, 0, this.Items.Count - 1)
                : null;
            set => this.hoveredIndex = value;
        }

        /// <summary>
        /// Creates a new dropdown state.
        /// </summary>
        /// <param name="items">The items in the dropdown menu.</param>
        public State(List<(T Item, string Label)> items)
        {
            this.Items = items;
        }
    }
}
