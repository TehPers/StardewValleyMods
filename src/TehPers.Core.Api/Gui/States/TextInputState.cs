using System;
using TehPers.Core.Api.Gui.Components;

namespace TehPers.Core.Api.Gui.States
{
    /// <summary>
    /// The state for a <see cref="TextInput"/>.
    /// </summary>
    public class TextInputState
    {
        private string text = string.Empty;
        private int anchorCursor;
        private int? selectionCursor;

        /// <summary>
        /// The text in the input.
        /// </summary>
        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                if (this.anchorCursor > this.text.Length)
                {
                    this.anchorCursor = this.text.Length;
                }

                if (this.selectionCursor is { } selectionCursor
                    && selectionCursor > this.text.Length)
                {
                    this.SelectionCursor = this.text.Length;
                }
            }
        }

        /// <summary>
        /// Whether the input is focused.
        /// </summary>
        public bool Focused { get; set; }

        /// <summary>
        /// The method of text insertion.
        /// </summary>
        public InsertMode InsertionMode { get; set; } = InsertMode.Insert;

        /// <summary>
        /// The anchor cursor. This is the primary one that's moved around.
        /// </summary>
        public int AnchorCursor
        {
            get => this.anchorCursor;
            set
            {
                if (this.selectionCursor == value)
                {
                    this.selectionCursor = null;
                }

                this.anchorCursor = Math.Clamp(value, 0, this.Text.Length);
            }
        }

        /// <summary>
        /// The selection cursor, if any text is selected. This is the one used whenever text
        /// is being selected, and may come before or after the anchor cursor.
        /// </summary>
        public int? SelectionCursor
        {
            get => this.selectionCursor;
            set
            {
                if (this.anchorCursor == value)
                {
                    this.selectionCursor = null;
                    return;
                }

                this.selectionCursor =
                    value is { } val ? Math.Clamp(val, 0, this.Text.Length) : null;
            }
        }

        /// <summary>
        /// Gets the range of selected text, if any.
        /// </summary>
        public (int Start, int End)? Selection
        {
            get
            {
                if (this.SelectionCursor is not { } selectionCursor)
                {
                    return null;
                }

                return selectionCursor > this.AnchorCursor
                    ? (this.AnchorCursor, selectionCursor)
                    : (selectionCursor, this.AnchorCursor);
            }
        }
    }
}
