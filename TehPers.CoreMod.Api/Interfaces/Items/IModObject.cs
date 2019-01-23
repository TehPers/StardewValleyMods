using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public interface IModObject {
        /// <summary>Gets the raw information that should be added to Data\ObjectInformation.</summary>
        /// <returns>The raw information string.</returns>
        string GetRawObjectInformation();

        /// <summary>Gets the data file this object's raw information should be added to.</summary>
        /// <returns>The name of the data file this object's entry should be added to.</returns>
        string GetDataSource();

        /// <summary>Overrides the drawing information to properly draw the object.</summary>
        /// <param name="info">The drawing information that should be updated.</param>
        void OverrideDraw(IDrawingInfo info);
    }
}