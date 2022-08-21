using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.Core.Gui.Extensions;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="ITextInput"/>
internal record TextInput
    (IGuiBuilder Builder, ITextInput.IState State, IInputHelper InputHelper) : BaseGuiComponent(
        Builder
    ), ITextInput
{
    private const string defaultInsertionCue = "cowboy_monsterhit";
    private const string defaultDeletionCue = "tinyWhip";
    private static readonly ImmutableDictionary<char, string?> allInsertionCues;

    static TextInput()
    {
        var insertionCuesBuilder = ImmutableDictionary.CreateBuilder<char, string?>();
        insertionCuesBuilder['$'] = "money";
        insertionCuesBuilder['*'] = "hammer";
        insertionCuesBuilder['+'] = "slimeHit";
        insertionCuesBuilder['<'] = "crystal";
        insertionCuesBuilder['='] = "coin";
        TextInput.allInsertionCues = insertionCuesBuilder.ToImmutable();
    }

    public SpriteFont Font { get; init; } = Game1.smallFont;
    public Color TextColor { get; init; } = Color.Black;
    public Color? CursorColor { get; init; }
    public Color? HighlightedTextColor { get; init; }
    public Color? HighlightedTextBackgroundColor { get; init; } = Color.DeepSkyBlue;
    public bool UnfocusOnEsc { get; init; } = true;
    public string? DefaultInsertionCue { get; init; } = TextInput.defaultInsertionCue;

    public ImmutableDictionary<char, string?> InsertionCues { get; init; } =
        TextInput.allInsertionCues;

    public string? DeletionCue { get; init; } = TextInput.defaultDeletionCue;
    public float LayerDepth { get; init; } = 1f;

    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        const string testString =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[{]}\\|;:'\",<.>/?`~ \t";
        return new GuiConstraints(MinSize: new GuiSize(0, this.Font.MeasureString(testString).Y));
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        // Focus handling
        if (e.Clicked(bounds, ClickType.Left))
        {
            this.State.Focused = true;
        }
        else if (e.IsReceiveClick(out _, out _))
        {
            this.State.Focused = false;
        }

        // Input handling
        if (this.State.Focused)
        {
            this.HandleInput(e);
        }

        // Drawing
        e.Draw(batch => batch.WithScissorRect(bounds, batch => this.Draw(batch, bounds)));
    }

    private void PlayInsertionCue(string text)
    {
        if (text.Length == 1 && this.InsertionCues.TryGetValue(text[0], out var overrideCue))
        {
            if (overrideCue is not null)
            {
                Game1.playSound(overrideCue);
            }
        }
        else
        {
            if (this.DefaultInsertionCue is not null)
            {
                Game1.playSound(this.DefaultInsertionCue);
            }
        }
    }

    private void PlayDeletionCue()
    {
        if (this.DeletionCue is not null)
        {
            Game1.playSound(this.DeletionCue);
        }
    }

    private void HandleInput(IGuiEvent e)
    {
        // Text input
        if (e.IsTextInput(out var text) && text.Length > 0)
        {
            // Get text selection
            if (this.State.Selection is not var (selectionStart, selectionEnd))
            {
                selectionStart = this.State.AnchorCursor;
                selectionEnd = this.State.InsertionMode switch
                {
                    ITextInput.InsertMode.Replace when this.State.AnchorCursor
                        < this.State.Text.Length => this.State.AnchorCursor + 1,
                    _ => this.State.AnchorCursor,
                };
            }

            // Build new text
            var newText = new StringBuilder(this.State.Text.Length + 1);
            newText.Append((string?)this.State.Text, 0, selectionStart);
            newText.Append(text);
            newText.Append(
                (string?)this.State.Text,
                selectionEnd,
                this.State.Text.Length - selectionEnd
            );
            this.State.Text = newText.ToString();

            // Update cursor
            this.State.AnchorCursor = selectionStart + text.Length;
            this.State.SelectionCursor = null;

            // Play sound effect if needed
            this.PlayInsertionCue(text);
        }

        // Editing
        if (e.IsKeyboardInput(out var key))
        {
            var ctrlPressed = this.InputHelper.IsDown(SButton.LeftControl)
                || this.InputHelper.IsDown(SButton.RightControl);
            var shiftPressed = this.InputHelper.IsDown(SButton.LeftShift)
                || this.InputHelper.IsDown(SButton.RightShift);
            var altPressed = this.InputHelper.IsDown(SButton.LeftAlt)
                || this.InputHelper.IsDown(SButton.RightAlt);

            switch (key)
            {
                case { } when altPressed:
                    {
                        break;
                    }
                case Keys.A when ctrlPressed && this.State.Text.Length > 0:
                    {
                        this.State.AnchorCursor = 0;
                        this.State.SelectionCursor = this.State.Text.Length;
                        break;
                    }
                case Keys.Escape when this.UnfocusOnEsc:
                    {
                        this.State.Focused = false;
                        break;
                    }
                case Keys.Left when shiftPressed:
                    {
                        var selectionCursor = this.State.SelectionCursor ?? this.State.AnchorCursor;
                        selectionCursor = TextInput.LeftCursorIndex(
                            selectionCursor,
                            this.State.Text,
                            ctrlPressed
                        );
                        this.State.SelectionCursor = selectionCursor;
                        break;
                    }
                case Keys.Left when this.State.Selection is var (selectionStart, _):
                    {
                        this.State.AnchorCursor = TextInput.LeftCursorIndex(
                            selectionStart,
                            this.State.Text,
                            ctrlPressed
                        );
                        this.State.SelectionCursor = null;
                        break;
                    }
                case Keys.Left:
                    {
                        this.State.AnchorCursor = TextInput.LeftCursorIndex(
                            this.State.AnchorCursor,
                            this.State.Text,
                            ctrlPressed
                        );
                        this.State.SelectionCursor = null;
                        break;
                    }
                case Keys.Right when shiftPressed:
                    {
                        var selectionCursor = this.State.SelectionCursor ?? this.State.AnchorCursor;
                        selectionCursor = TextInput.RightCursorIndex(
                            selectionCursor,
                            this.State.Text,
                            ctrlPressed
                        );
                        this.State.SelectionCursor = selectionCursor;
                        break;
                    }
                case Keys.Right when this.State.Selection is var (_, selectionEnd):
                    {
                        this.State.AnchorCursor = TextInput.RightCursorIndex(
                            selectionEnd,
                            this.State.Text,
                            ctrlPressed
                        );
                        this.State.SelectionCursor = null;
                        break;
                    }
                case Keys.Right:
                    {
                        this.State.AnchorCursor = TextInput.RightCursorIndex(
                            this.State.AnchorCursor,
                            this.State.Text,
                            ctrlPressed
                        );
                        this.State.SelectionCursor = null;
                        break;
                    }
                case Keys.Back or Keys.Delete
                    when this.State.Selection is var (selectionStart, selectionEnd):
                    {
                        this.State.Text = this.State.Text.Remove(
                            selectionStart,
                            selectionEnd - selectionStart
                        );
                        this.State.AnchorCursor = selectionStart;
                        this.State.SelectionCursor = null;
                        this.PlayDeletionCue();
                        break;
                    }
                case Keys.Back:
                    {
                        var deleteFrom = TextInput.LeftCursorIndex(
                            this.State.AnchorCursor,
                            this.State.Text,
                            ctrlPressed
                        );
                        this.State.Text = this.State.Text.Remove(
                            deleteFrom,
                            this.State.AnchorCursor - deleteFrom
                        );
                        this.State.AnchorCursor = deleteFrom;
                        this.PlayDeletionCue();
                        break;
                    }
                case Keys.Delete:
                    {
                        var deleteTo = TextInput.RightCursorIndex(
                            this.State.AnchorCursor,
                            this.State.Text,
                            ctrlPressed
                        );
                        this.State.Text = this.State.Text.Remove(
                            this.State.AnchorCursor,
                            deleteTo - this.State.AnchorCursor
                        );
                        this.PlayDeletionCue();
                        break;
                    }
                case Keys.Home or Keys.Up when shiftPressed:
                    {
                        this.State.SelectionCursor = 0;
                        break;
                    }
                case Keys.Home or Keys.Up:
                    {
                        this.State.AnchorCursor = 0;
                        this.State.SelectionCursor = null;
                        break;
                    }
                case Keys.End or Keys.Down when shiftPressed:
                    {
                        this.State.SelectionCursor = this.State.Text.Length;
                        break;
                    }
                case Keys.End or Keys.Down:
                    {
                        this.State.AnchorCursor = this.State.Text.Length;
                        this.State.SelectionCursor = null;
                        break;
                    }
                case Keys.Insert:
                    {
                        this.State.InsertionMode = this.State.InsertionMode switch
                        {
                            ITextInput.InsertMode.Insert => ITextInput.InsertMode.Replace,
                            ITextInput.InsertMode.Replace => ITextInput.InsertMode.Insert,
                            _ => throw new InvalidOperationException(
                                $"Invalid insertion mode: {this.State.InsertionMode}"
                            ),
                        };
                        break;
                    }
            }
        }
    }

    private static int LeftCursorIndex(int cursor, string text, bool ctrlPressed)
    {
        if (cursor == 0)
        {
            return 0;
        }

        if (!ctrlPressed)
        {
            return cursor - 1;
        }

        var contents = text.AsSpan();

        // Skip any whitespace (going backwards)
        while (cursor > 0 && char.IsWhiteSpace(contents[cursor - 1]))
        {
            cursor--;
        }

        // Skip any non-whitespace (going backwards)
        while (cursor > 0 && !char.IsWhiteSpace(contents[cursor - 1]))
        {
            cursor--;
        }

        return cursor;
    }

    private static int RightCursorIndex(int cursor, string text, bool ctrlPressed)
    {
        if (cursor == text.Length)
        {
            return text.Length;
        }

        if (!ctrlPressed)
        {
            return cursor + 1;
        }

        var contents = text.AsSpan();

        // Skip any non-whitespace (going forwards)
        while (cursor < contents.Length && !char.IsWhiteSpace(contents[cursor]))
        {
            cursor++;
        }

        // Skip any whitespace (going forwards)
        while (cursor < contents.Length && char.IsWhiteSpace(contents[cursor]))
        {
            cursor++;
        }

        return cursor;
    }

    private void Draw(SpriteBatch batch, Rectangle bounds)
    {
        // Get selected text
        if (this.State.Selection is not var (selectionStart, selectionEnd))
        {
            selectionStart = this.State.AnchorCursor;
            selectionEnd = this.State.AnchorCursor;
        }

        // Before selection
        var xPos = bounds.X;
        if (selectionStart > 0)
        {
            var beforeCursor = this.State.Text[..selectionStart];
            batch.DrawString(this.Font, beforeCursor, new(bounds.X, bounds.Y), this.TextColor);
            xPos += (int)Math.Ceiling(this.Font.MeasureString(beforeCursor).X);
        }

        // Selection
        var shouldDrawCursor = this.State.Focused;
        if (selectionStart != selectionEnd)
        {
            var selection = this.State.Text[selectionStart..selectionEnd];
            var selectionWidth = (int)Math.Ceiling(this.Font.MeasureString(selection).X);

            // Draw selection background
            if (this.HighlightedTextBackgroundColor is { } highlightColor)
            {
                batch.Draw(
                    DrawUtils.WhitePixel,
                    new Rectangle(xPos, bounds.Top, selectionWidth, bounds.Height),
                    highlightColor
                );
            }

            // Draw the cursor
            if (shouldDrawCursor)
            {
                var cursorPos = this.State.AnchorCursor == selectionStart
                    ? xPos
                    : xPos + selectionWidth;
                batch.Draw(
                    DrawUtils.WhitePixel,
                    new Rectangle(cursorPos, bounds.Top, 2, bounds.Height),
                    this.CursorColor ?? this.TextColor
                );
            }

            // Draw selected text
            batch.DrawString(
                this.Font,
                selection,
                new(xPos, bounds.Y),
                this.HighlightedTextColor ?? this.TextColor,
                0f,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                this.LayerDepth
            );

            xPos += selectionWidth;
        }
        else
        {
            // Draw the cursor
            if (shouldDrawCursor)
            {
                batch.Draw(
                    DrawUtils.WhitePixel,
                    new Rectangle(xPos, bounds.Top, 2, bounds.Height),
                    this.CursorColor ?? this.TextColor
                );
            }
        }

        // After selection
        if (selectionEnd < this.State.Text.Length)
        {
            var afterCursor = this.State.Text[selectionEnd..];
            batch.DrawString(this.Font, afterCursor, new(xPos, bounds.Y), this.TextColor);
        }
    }

    /// <inheritdoc />
    public ITextInput WithInputHelper(IInputHelper inputHelper)
    {
        return this with {InputHelper = inputHelper};
    }

    /// <inheritdoc />
    public ITextInput WithFont(SpriteFont font)
    {
        return this with {Font = font};
    }

    /// <inheritdoc />
    public ITextInput WithTextColor(Color color)
    {
        return this with {TextColor = color};
    }

    /// <inheritdoc />
    public ITextInput WithCursorColor(Color? color)
    {
        return this with {CursorColor = color};
    }

    /// <inheritdoc />
    public ITextInput WithHighlightedTextColor(Color? color)
    {
        return this with {HighlightedTextColor = color};
    }

    /// <inheritdoc />
    public ITextInput WithHighlightedTextBackgroundColor(Color? color)
    {
        return this with {HighlightedTextBackgroundColor = color};
    }

    /// <inheritdoc />
    public ITextInput WithUnfocusOnEsc(bool unfocus)
    {
        return this with {UnfocusOnEsc = unfocus};
    }

    /// <inheritdoc />
    public ITextInput WithDefaultInsertionCue(string? insertionCue)
    {
        return this with {DefaultInsertionCue = insertionCue};
    }

    /// <inheritdoc />
    public ITextInput WithInsertionCues(IDictionary<char, string?> insertionCues)
    {
        return this with {InsertionCues = insertionCues.ToImmutableDictionary()};
    }

    /// <inheritdoc />
    public ITextInput WithLayerDepth(float layerDepth)
    {
        return this with {LayerDepth = layerDepth};
    }

    /// <inheritdoc />
    public ITextInput WithState(ITextInput.IState state)
    {
        return this with {State = state};
    }
}
