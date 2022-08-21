using Microsoft.Xna.Framework;

namespace TehPers.Core.Gui.Extensions.Drawing;

/// <summary>
/// Centers the sprite on the given position.
/// </summary>
public record CenterDrawOrigin() : DrawOrigin(Vector2.One * 2f, Vector2.One);
