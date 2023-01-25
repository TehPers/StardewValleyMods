using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component with simple functionality.
/// </summary>
public interface ISimpleComponent : IGuiComponent
{
    /// <summary>
    /// Sets the constraints for this component.
    /// </summary>
    /// <param name="constraints">The new constraints.</param>
    /// <returns>The resulting component.</returns>
    ISimpleComponent WithConstraints(IGuiConstraints constraints);

    /// <summary>
    /// Sets the action to execute when drawing this component.
    /// </summary>
    /// <param name="draw">The new action to execute when drawing this component.</param>
    /// <returns>The resulting component.</returns>
    ISimpleComponent WithDrawAction(Action<SpriteBatch, Rectangle> draw);
}
