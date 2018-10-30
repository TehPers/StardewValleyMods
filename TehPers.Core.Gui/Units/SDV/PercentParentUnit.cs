using TehPers.Core.Gui.Units.Base;

namespace TehPers.Core.Gui.Units.SDV {
    public class PercentParentUnit : IGuiUnit {
        /// <inheritdoc />
        public float Quantity { get; }

        public PercentParentUnit(float quantity) {
            this.Quantity = quantity;
        }

        /// <inheritdoc />
        public float Resolve(GuiInfo info) {
            return this.Quantity * info.ParentUnits;
        }

        /// <inheritdoc />
        public IUnit<GuiInfo> Negate() {
            return new PercentParentUnit(-this.Quantity);
        }

        /// <inheritdoc />
        public bool TryAdd(IUnit<GuiInfo> other, out IUnit<GuiInfo> sum) {
            if (other is PercentParentUnit) {
                sum = new PercentParentUnit(this.Quantity + other.Quantity);
                return true;
            }

            sum = default;
            return false;
        }

        /// <inheritdoc />
        public IUnit<GuiInfo> Multiply(float scalar) {
            return new PercentParentUnit(this.Quantity * scalar);
        }
    }
}