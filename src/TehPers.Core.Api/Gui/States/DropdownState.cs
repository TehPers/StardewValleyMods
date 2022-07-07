using System;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.Api.Gui.States
{
    /// <summary>
    /// The state of a dropdown.
    /// </summary>
    /// <typeparam name="T">The type of item the dropdown holds.</typeparam>
    public class DropdownState<T>
    {
        private int index;
        private int maxVisibleItems = 4;
        private int topVisibleIndex;
        private int? hoveredIndex;

        /// <summary>
        /// Gets or sets whether the dropdown is dropped.
        /// </summary>
        public bool Dropped { get; set; }

        /// <summary>
        /// Gets or sets the items in the dropdown.
        /// </summary>
        public List<(T Item, string Label)> Items { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get => this.Items.Any() ? Math.Clamp(this.index, 0, this.Items.Count - 1) : 0;
            set => this.index = value;
        }

        /// <summary>
        /// The selected item, if any.
        /// </summary>
        public (T Item, string Label)? Selected => this.Items.Any() ? this.Items[this.SelectedIndex] : null;

        /// <summary>
        /// The maximum number of items that should be visible at a time.
        /// </summary>
        public int MaxVisibleItems
        {
            get => this.maxVisibleItems;
            set => this.maxVisibleItems = Math.Min(1, value);
        }

        /// <summary>
        /// The index of the top item that is currently visible.
        /// </summary>
        public int TopVisibleIndex
        {
            get => this.Items.Count > this.MaxVisibleItems ? Math.Clamp(this.topVisibleIndex, 0, this.Items.Count - this.MaxVisibleItems) : 0;
            set => this.topVisibleIndex = value;
        }

        /// <summary>
        /// The index of the hovered item.
        /// </summary>
        public int? HoveredIndex
        {
            get => this.hoveredIndex is { } hoveredIndex && this.Dropped && this.Items.Any() ? Math.Clamp(hoveredIndex, 0, this.Items.Count - 1) : null;
            set => this.hoveredIndex = value;
        }

        /// <summary>
        /// Creates a new dropdown state.
        /// </summary>
        /// <param name="items">The items in the dropdown menu.</param>
        public DropdownState(List<(T Item, string Label)> items)
        {
            this.Items = items;
        }
    }
}
