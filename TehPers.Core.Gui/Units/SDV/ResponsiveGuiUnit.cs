using System.Collections.Generic;
using TehPers.Core.Gui.Units.Base;

namespace TehPers.Core.Gui.Units.SDV {
    public class ResponsiveGuiUnit : ResponsiveUnit<GuiInfo> {
        public ResponsiveGuiUnit(params IUnit<GuiInfo>[] units) : base(units) { }
        public ResponsiveGuiUnit(IEnumerable<IUnit<GuiInfo>> units) : base(units) { }

        /// <summary>Responsive unit which resolves to 0.</summary>
        public new static ResponsiveGuiUnit Zero { get; } = new ResponsiveGuiUnit();

        /// <summary>Responsive unit equal to half the parent's unit of the same type along the same axis.</summary>
        public static ResponsiveGuiUnit Half { get; } = new ResponsiveGuiUnit(new PercentParentUnit(0.5f));

        /// <summary>Responsive unit centered in the parent along the same axis.</summary>
        public static ResponsiveGuiUnit Centered { get; } = new ResponsiveGuiUnit(new PercentParentUnit(1f), new PercentParentSizeUnit(0.5f), new PercentSizeUnit(-0.5f));

        /// <summary>Responsive unit equal to the parent's unit of the same type along the same axis.</summary>
        public static ResponsiveGuiUnit SameAsParent { get; } = new ResponsiveGuiUnit(new PercentParentUnit(1f));
    }
}
