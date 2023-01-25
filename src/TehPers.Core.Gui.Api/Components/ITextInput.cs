using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A single-line text input component.
/// </summary>
public interface ITextInput : IGuiComponent, IWithLayerDepth<ITextInput>,
    IWithState<ITextInput.IState, ITextInput>
{
    /// <summary>
    /// Sets the input helper to use for tracking input.
    /// </summary>
    /// <param name="inputHelper">The input helper.</param>
    /// <returns>The resulting component</returns>
    ITextInput WithInputHelper(IInputHelper inputHelper);

    /// <summary>
    /// Sets the text font.
    /// </summary>
    /// <param name="font">The new text font.</param>
    /// <returns>The resulting component.</returns>
    ITextInput WithFont(SpriteFont font);

    /// <summary>
    /// Sets the text color.
    /// </summary>
    /// <param name="color">The new text color.</param>
    /// <returns>The resulting component.</returns>
    ITextInput WithTextColor(Color color);

    /// <summary>
    /// Sets the cursor color.
    /// </summary>
    /// <param name="color">The new cursor color.</param>
    /// <returns>The resulting component.</returns>
    ITextInput WithCursorColor(Color? color);

    /// <summary>
    /// Sets the highlighted text color.
    /// </summary>
    /// <param name="color">The new highlighted text color.</param>
    /// <returns>The resulting component.</returns>
    ITextInput WithHighlightedTextColor(Color? color);

    /// <summary>
    /// Sets the highlighted text background color.
    /// </summary>
    /// <param name="color">The new highighted text background color.</param>
    /// <returns>The resulting component.</returns>
    ITextInput WithHighlightedTextBackgroundColor(Color? color);

    /// <summary>
    /// Sets whether the text input should be unfocused when the ESC key is pressed.
    /// </summary>
    /// <param name="unfocus">Whether the text input should be unfocused.</param>
    /// <returns>The resulting component.</returns>
    ITextInput WithUnfocusOnEsc(bool unfocus);

    /// <summary>
    /// Sets the default cue that is played when text is inserted.
    /// </summary>
    /// <param name="insertionCue">The new default cue to play.</param>
    /// <returns>Ther esulting component.</returns>
    ITextInput WithDefaultInsertionCue(string? insertionCue);

    /// <summary>
    /// Sets the cues to play when specific characters are typed.
    /// </summary>
    /// <param name="insertionCues"></param>
    /// <returns></returns>
    ITextInput WithInsertionCues(IDictionary<char, string?> insertionCues);

    /// <summary>
    /// The state for a text input.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// The text in the input.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// The anchor cursor. This is the primary one that's moved around.
        /// </summary>
        int AnchorCursor { get; set; }

        /// <summary>
        /// The selection cursor, if any text is selected. This is the one used whenever text
        /// is being selected, and may come before or after the anchor cursor.
        /// </summary>
        int? SelectionCursor { get; set; }

        /// <summary>
        /// Whether the input is focused.
        /// </summary>
        bool Focused { get; set; }

        /// <summary>
        /// The method of text insertion.
        /// </summary>
        InsertMode InsertionMode { get; set; }

        /// <summary>
        /// Gets the range of selected text, if any.
        /// </summary>
        (int Start, int End)? Selection
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

    /// <inheritdoc />
    public class State : IState
    {
        private string text = string.Empty;
        private int anchorCursor;
        private int? selectionCursor;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool Focused { get; set; }

        /// <inheritdoc />
        public InsertMode InsertionMode { get; set; } = InsertMode.Insert;

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <summary>
    /// Modes of text insertion.
    /// </summary>
    public enum InsertMode
    {
        /// <summary>
        /// Inserts the text at the cursor position.
        /// </summary>
        Insert,

        /// <summary>
        /// Replaces the text at the cursor position.
        /// </summary>
        Replace,
    }
}
