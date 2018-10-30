using TehPers.FestiveSlimes.Drawing;
using SObject = StardewValley.Object;

namespace TehPers.FestiveSlimes.Items {
    public interface IItemDescription {
        /// <summary>Gets the raw information that should be added to Data\ObjectInformation.</summary>
        /// <returns>The raw information string.</returns>
        string GetRawInformation();

        /// <summary>Creates an instance of this object with the specified parent sheet index.</summary>
        /// <param name="index">The parent sheet index of the object to create.</param>
        /// <returns>A new instance of <see cref="SObject"/> with the given index. It should not be a subclass of <see cref="SObject"/>.</returns>
        SObject CreateObject(int index);

        /// <summary>Overrides the drawing information to properly draw the object.</summary>
        /// <param name="info">The drawing information that should be updated.</param>
        void OverrideTexture(DrawingInfo info);
    }
}