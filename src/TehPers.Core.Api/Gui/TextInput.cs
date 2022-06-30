using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Immutable;
using System.Text;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A single-line text input component.
    /// </summary>
    public class TextInput : IGuiComponent
    {
        private static readonly Texture2D whitePixel;

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

        static TextInput()
        {
            TextInput.whitePixel = new(Game1.spriteBatch.GraphicsDevice, 1, 1);
            TextInput.whitePixel.SetData(new[] {Color.White});

            var insertionCuesBuilder = ImmutableDictionary.CreateBuilder<char, string?>();
            insertionCuesBuilder['$'] = "money";
            insertionCuesBuilder['*'] = "hammer";
            insertionCuesBuilder['+'] = "slimeHit";
            insertionCuesBuilder['<'] = "crystal";
            insertionCuesBuilder['='] = "coin";
            TextInput.AllInsertionCues = insertionCuesBuilder.ToImmutable();
        }

        private readonly State state;
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
        /// The background color of the highlighted text.
        /// </summary>
        public Color? HighlightedTextBackgroundColor { get; init; } = Color.DeepSkyBlue;

        /// <summary>
        /// The color of the highlighted text.
        /// </summary>
        public Color? HighlightedTextColor { get; init; } = null;

        /// <summary>
        /// Whether this component should lose focus when ESC is received.
        /// </summary>
        public bool UnfocusOnEsc { get; init; } = true;
        
        /// <summary>
        /// The sound cue to play when text is typed and no specific cue overrides it.
        /// </summary>
        public string? InsertionCue { get; init; } = TextInput.DefaultInsertionCue;

        /// <summary>
        /// The sound cue to play when specific characters are typed instead.
        /// </summary>
        public ImmutableDictionary<char, string?> OverrideInsertionCues { get; init; } =
            TextInput.AllInsertionCues;

        /// <summary>
        /// The sound cue to play when text is deleted.
        /// </summary>
        public string? DeletionCue { get; init; } = TextInput.DefaultDeletionCue;

        /// <summary>
        /// Creates a new <see cref="TextInput"/>.
        /// </summary>
        /// <param name="state">The state of the text box.</param>
        /// <param name="inputHelper">The input helper.</param>
        public TextInput(State state, IInputHelper inputHelper)
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
            e.Draw(batch => this.Draw(batch, bounds));
        }

        private void PlayInsertionCue(string text)
        {
            if (text.Length == 1 && this.OverrideInsertionCues.TryGetValue(text[0], out var overrideCue))
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
                        TextInput.whitePixel,
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
                        TextInput.whitePixel,
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
                        TextInput.whitePixel,
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

        /// <summary>
        /// The state for a <see cref="TextInput"/>.
        /// </summary>
        public class State
        {
            /// <summary>
            /// The text in the input.
            /// </summary>
            public string Text { get; set; } = string.Empty;

            /// <summary>
            /// Whether the input is focused.
            /// </summary>
            public bool Focused { get; set; }

            /// <summary>
            /// The method of text insertion.
            /// </summary>
            public InsertMode InsertionMode { get; set; } = InsertMode.Insert;

            private int anchorCursor;

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

            private int? selectionCursor;

            /// <summary>
            /// The selection cursor, if any text is selected. This is the one used whenever text
            /// is being selected.
            /// </summary>
            public int? SelectionCursor
            {
                get => this.selectionCursor;
                set
                {
                    if (value == this.AnchorCursor)
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
}
