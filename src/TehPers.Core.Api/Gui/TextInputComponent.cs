using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Immutable;
using System.Text;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A single-line text input component.
    /// </summary>
    internal record TextInputComponent : IGuiComponent
    {
        /// <summary>
        /// The default sound cue to play when text is inserted.
        /// </summary>
        public static string DefaultInsertionCue => "cowboy_monsterhit";

        /// <summary>
        /// The default sound cue to play when text is deleted.
        /// </summary>
        public static string DefaultDeletionCue => "tinyWhip";

        /// <summary>
        /// All special text insertion cues.
        /// </summary>
        public static ImmutableDictionary<char, string?> AllInsertionCues { get; }

        static TextInputComponent()
        {
            var insertionCuesBuilder = ImmutableDictionary.CreateBuilder<char, string?>();
            insertionCuesBuilder['$'] = "money";
            insertionCuesBuilder['*'] = "hammer";
            insertionCuesBuilder['+'] = "slimeHit";
            insertionCuesBuilder['<'] = "crystal";
            insertionCuesBuilder['='] = "coin";
            TextInputComponent.AllInsertionCues = insertionCuesBuilder.ToImmutable();
        }

        private readonly TextInputState state;
        private readonly IInputHelper inputHelper;

        /// <summary>
        /// The font of the text in the input.
        /// </summary>
        public SpriteFont Font { get; init; } = Game1.smallFont;

        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color TextColor { get; init; } = Color.Black;

        /// <summary>
        /// The color of the cursor.
        /// </summary>
        public Color? CursorColor { get; init; } = null;

        /// <summary>
        /// The color of the highlighted text.
        /// </summary>
        public Color? HighlightedTextColor { get; init; } = null;

        /// <summary>
        /// The background color of the highlighted text.
        /// </summary>
        public Color? HighlightedTextBackgroundColor { get; init; } = Color.DeepSkyBlue;

        /// <summary>
        /// Whether this component should lose focus when ESC is received.
        /// </summary>
        public bool UnfocusOnEsc { get; init; } = true;

        /// <summary>
        /// The sound cue to play when text is typed and no specific cue overrides it.
        /// </summary>
        public string? InsertionCue { get; init; } = TextInputComponent.DefaultInsertionCue;

        /// <summary>
        /// The sound cue to play when specific characters are typed instead.
        /// </summary>
        public ImmutableDictionary<char, string?> OverrideInsertionCues { get; init; } =
            TextInputComponent.AllInsertionCues;

        /// <summary>
        /// The sound cue to play when text is deleted.
        /// </summary>
        public string? DeletionCue { get; init; } = TextInputComponent.DefaultDeletionCue;

        /// <summary>
        /// Creates a new <see cref="TextInputComponent"/>.
        /// </summary>
        /// <param name="state">The state of the text input.</param>
        /// <param name="inputHelper">The input helper.</param>
        public TextInputComponent(TextInputState state, IInputHelper inputHelper)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            const string testString =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[{]}\\|;:'\",<.>/?`~ \t";
            return new()
            {
                MinSize = new(0, this.Font.MeasureString(testString).Y),
            };
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            // Focus handling
            if (e.Clicked(bounds, ClickType.Left))
            {
                this.state.Focused = true;
            }

            // Input handling
            if (this.state.Focused)
            {
                this.HandleInput(e);
            }

            // Drawing
            e.Draw(batch => batch.WithScissorRect(bounds, batch => this.Draw(batch, bounds)));
        }

        private void PlayInsertionCue(string text)
        {
            if (text.Length == 1
                && this.OverrideInsertionCues.TryGetValue(text[0], out var overrideCue))
            {
                if (overrideCue is not null)
                {
                    Game1.playSound(overrideCue);
                }
            }
            else
            {
                if (this.InsertionCue is not null)
                {
                    Game1.playSound(this.InsertionCue);
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

        private void HandleInput(GuiEvent e)
        {
            // Text input
            if (e is GuiEvent.TextInput(var text) && text.Length > 0)
            {
                // Get text selection
                if (this.state.Selection is not var (selectionStart, selectionEnd))
                {
                    selectionStart = this.state.AnchorCursor;
                    selectionEnd = this.state.InsertionMode switch
                    {
                        InsertMode.Replace when this.state.AnchorCursor < this.state.Text.Length =>
                            this.state.AnchorCursor + 1,
                        _ => this.state.AnchorCursor,
                    };
                }

                // Build new text
                var newText = new StringBuilder(this.state.Text.Length + 1);
                newText.Append(this.state.Text, 0, selectionStart);
                newText.Append(text);
                newText.Append(
                    this.state.Text,
                    selectionEnd,
                    this.state.Text.Length - selectionEnd
                );
                this.state.Text = newText.ToString();

                // Update cursor
                this.state.AnchorCursor = selectionStart + text.Length;
                this.state.SelectionCursor = null;

                // Play sound effect if needed
                this.PlayInsertionCue(text);
            }

            // Editing
            if (e is GuiEvent.KeyboardInput(var key))
            {
                var ctrlPressed = this.inputHelper.IsDown(SButton.LeftControl)
                    || this.inputHelper.IsDown(SButton.RightControl);
                var shiftPressed = this.inputHelper.IsDown(SButton.LeftShift)
                    || this.inputHelper.IsDown(SButton.RightShift);
                var altPressed = this.inputHelper.IsDown(SButton.LeftAlt)
                    || this.inputHelper.IsDown(SButton.RightAlt);

                switch (key)
                {
                    case { } when altPressed:
                        {
                            break;
                        }
                    case Keys.A when ctrlPressed && this.state.Text.Length > 0:
                        {
                            this.state.AnchorCursor = 0;
                            this.state.SelectionCursor = this.state.Text.Length;
                            break;
                        }
                    case Keys.Escape when this.UnfocusOnEsc:
                        {
                            this.state.Focused = false;
                            break;
                        }
                    case Keys.Left when shiftPressed:
                        {
                            var selectionCursor =
                                this.state.SelectionCursor ?? this.state.AnchorCursor;
                            selectionCursor = this.LeftCursorIndex(
                                selectionCursor,
                                this.state.Text,
                                ctrlPressed
                            );
                            this.state.SelectionCursor = selectionCursor;
                            break;
                        }
                    case Keys.Left when this.state.Selection is var (selectionStart, _):
                        {
                            this.state.AnchorCursor = this.LeftCursorIndex(
                                selectionStart,
                                this.state.Text,
                                ctrlPressed
                            );
                            this.state.SelectionCursor = null;
                            break;
                        }
                    case Keys.Left:
                        {
                            this.state.AnchorCursor = this.LeftCursorIndex(
                                this.state.AnchorCursor,
                                this.state.Text,
                                ctrlPressed
                            );
                            this.state.SelectionCursor = null;
                            break;
                        }
                    case Keys.Right when shiftPressed:
                        {
                            var selectionCursor =
                                this.state.SelectionCursor ?? this.state.AnchorCursor;
                            selectionCursor = this.RightCursorIndex(
                                selectionCursor,
                                this.state.Text,
                                ctrlPressed
                            );
                            this.state.SelectionCursor = selectionCursor;
                            break;
                        }
                    case Keys.Right when this.state.Selection is var (_, selectionEnd):
                        {
                            this.state.AnchorCursor = this.RightCursorIndex(
                                selectionEnd,
                                this.state.Text,
                                ctrlPressed
                            );
                            this.state.SelectionCursor = null;
                            break;
                        }
                    case Keys.Right:
                        {
                            this.state.AnchorCursor = this.RightCursorIndex(
                                this.state.AnchorCursor,
                                this.state.Text,
                                ctrlPressed
                            );
                            this.state.SelectionCursor = null;
                            break;
                        }
                    case Keys.Back or Keys.Delete
                        when this.state.Selection is var (selectionStart, selectionEnd):
                        {
                            this.state.Text = this.state.Text.Remove(
                                selectionStart,
                                selectionEnd - selectionStart
                            );
                            this.state.AnchorCursor = selectionStart;
                            this.state.SelectionCursor = null;
                            this.PlayDeletionCue();
                            break;
                        }
                    case Keys.Back:
                        {
                            var deleteFrom = this.LeftCursorIndex(
                                this.state.AnchorCursor,
                                this.state.Text,
                                ctrlPressed
                            );
                            this.state.Text = this.state.Text.Remove(
                                deleteFrom,
                                this.state.AnchorCursor - deleteFrom
                            );
                            this.state.AnchorCursor = deleteFrom;
                            this.PlayDeletionCue();
                            break;
                        }
                    case Keys.Delete:
                        {
                            var deleteTo = this.RightCursorIndex(
                                this.state.AnchorCursor,
                                this.state.Text,
                                ctrlPressed
                            );
                            this.state.Text = this.state.Text.Remove(
                                this.state.AnchorCursor,
                                deleteTo - this.state.AnchorCursor
                            );
                            this.PlayDeletionCue();
                            break;
                        }
                    case Keys.Home or Keys.Up when shiftPressed:
                        {
                            this.state.SelectionCursor = 0;
                            break;
                        }
                    case Keys.Home or Keys.Up:
                        {
                            this.state.AnchorCursor = 0;
                            this.state.SelectionCursor = null;
                            break;
                        }
                    case Keys.End or Keys.Down when shiftPressed:
                        {
                            this.state.SelectionCursor = this.state.Text.Length;
                            break;
                        }
                    case Keys.End or Keys.Down:
                        {
                            this.state.AnchorCursor = this.state.Text.Length;
                            this.state.SelectionCursor = null;
                            break;
                        }
                    case Keys.Insert:
                        {
                            this.state.InsertionMode = this.state.InsertionMode switch
                            {
                                InsertMode.Insert => InsertMode.Replace,
                                InsertMode.Replace => InsertMode.Insert,
                                _ => throw new InvalidOperationException(
                                    $"Invalid insertion mode: {this.state.InsertionMode}"
                                ),
                            };
                            break;
                        }
                }
            }
        }

        private int LeftCursorIndex(int cursor, string text, bool ctrlPressed)
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

        private int RightCursorIndex(int cursor, string text, bool ctrlPressed)
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
            if (this.state.Selection is not var (selectionStart, selectionEnd))
            {
                selectionStart = this.state.AnchorCursor;
                selectionEnd = this.state.AnchorCursor;
            }

            // Before selection
            var xPos = bounds.X;
            if (selectionStart > 0)
            {
                var beforeCursor = this.state.Text[..selectionStart];
                batch.DrawString(this.Font, beforeCursor, new(bounds.X, bounds.Y), this.TextColor);
                xPos += (int)Math.Ceiling(this.Font.MeasureString(beforeCursor).X);
            }

            // Selection
            var shouldDrawCursor = this.state.Focused;
            if (selectionStart != selectionEnd)
            {
                var selection = this.state.Text[selectionStart..selectionEnd];
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
                    var cursorPos = this.state.AnchorCursor == selectionStart
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
                    this.HighlightedTextColor ?? this.TextColor
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
            if (selectionEnd < this.state.Text.Length)
            {
                var afterCursor = this.state.Text[selectionEnd..];
                batch.DrawString(this.Font, afterCursor, new(xPos, bounds.Y), this.TextColor);
            }
        }
    }
}
