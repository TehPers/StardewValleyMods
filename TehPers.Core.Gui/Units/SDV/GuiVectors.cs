using TehPers.Core.Gui.Units.Base;

namespace TehPers.Core.Gui.Units.SDV {
    public static class GuiVectors {
        /// <summary>Responsive vector equal to the zero vector.</summary>
        public static ResponsiveVector2<GuiInfo> Zero { get; } = new ResponsiveVector2<GuiInfo>();

        /// <summary>Responsive vector equal to half the parent's units of the same type.</summary>
        public static ResponsiveVector2<GuiInfo> Half { get; } = new ResponsiveVector2<GuiInfo>(ResponsiveGuiUnit.Half, ResponsiveGuiUnit.Half);

        /// <summary>Responsive vector centered in the parent.</summary>
        public static ResponsiveVector2<GuiInfo> Centered { get; } = new ResponsiveVector2<GuiInfo>(ResponsiveGuiUnit.Centered, ResponsiveGuiUnit.Centered);

        /// <summary>Responsive vector equal to the parent's units of the same type.</summary>
        public static ResponsiveVector2<GuiInfo> SameAsParent { get; } = new ResponsiveVector2<GuiInfo>(ResponsiveGuiUnit.SameAsParent, ResponsiveGuiUnit.SameAsParent);
    }
}