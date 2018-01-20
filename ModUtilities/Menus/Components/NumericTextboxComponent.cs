using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModUtilities.Menus.Components.Interfaces;

namespace ModUtilities.Menus.Components {
    public class NumericTextboxComponent : TextboxComponent, IValueComponent<byte>, IValueComponent<ushort>, IValueComponent<short>, IValueComponent<uint>, IValueComponent<int>, IValueComponent<ulong>, IValueComponent<long>, IValueComponent<float>, IValueComponent<double>, IValueComponent<sbyte> {
        public double? Minimum { get; set; } = null;
        public double? Maximum { get; set; } = null;
        public bool AllowDecimal { get; set; } = true;
        public double Value {
            get => double.TryParse(this.Text, out double value) ? value : (this.Minimum ?? 0);
            set => this.Text = value.ToString(CultureInfo.CurrentCulture);
        }

        protected override bool IsValidText(string newText) {
            if (this.AllowDecimal)
                return double.TryParse(newText, out double dVal) && this.Minimum <= dVal && dVal <= this.Maximum;
            return long.TryParse(newText, out long iVal) && this.Minimum <= iVal && iVal <= this.Maximum;
        }

        #region IValueComponent
        #region SetValue
        void IValueComponent<byte>.SetValue(byte value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<sbyte>.SetValue(sbyte value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<ushort>.SetValue(ushort value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<short>.SetValue(short value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<uint>.SetValue(uint value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<int>.SetValue(int value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<ulong>.SetValue(ulong value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<long>.SetValue(long value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<float>.SetValue(float value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        void IValueComponent<double>.SetValue(double value) => this.Text = value.ToString(CultureInfo.CurrentCulture);
        #endregion

        #region GetValue
        double IValueComponent<double>.GetValue() => double.TryParse(this.Text, out double value) ? value : default;
        float IValueComponent<float>.GetValue() => float.TryParse(this.Text, out float value) ? value : default;
        long IValueComponent<long>.GetValue() => long.TryParse(this.Text, out long value) ? value : default;
        ulong IValueComponent<ulong>.GetValue() => ulong.TryParse(this.Text, out ulong value) ? value : default;
        int IValueComponent<int>.GetValue() => int.TryParse(this.Text, out int value) ? value : default;
        uint IValueComponent<uint>.GetValue() => uint.TryParse(this.Text, out uint value) ? value : default;
        short IValueComponent<short>.GetValue() => short.TryParse(this.Text, out short value) ? value : default;
        ushort IValueComponent<ushort>.GetValue() => ushort.TryParse(this.Text, out ushort value) ? value : default;
        sbyte IValueComponent<sbyte>.GetValue() => sbyte.TryParse(this.Text, out sbyte value) ? value : default;
        byte IValueComponent<byte>.GetValue() => byte.TryParse(this.Text, out byte value) ? value : default;
        #endregion
        #endregion
    }
}
