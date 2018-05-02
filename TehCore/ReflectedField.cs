using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FishingOverhaul {
    public class ReflectedField<TObject, TField> {
        public FieldInfo Field { get; }
        public TObject Owner { get; }

        public TField Value {
            get => (TField) this.Field.GetValue(this.Owner);
            set => this.Field.SetValue(this.Owner, value);
        }

        public ReflectedField(TObject owner, string field) {
            this.Field = typeof(TObject).GetFields().FirstOrDefault(f => f.Name == field && f.FieldType == typeof(TField));
            this.Owner = owner;

            if (this.Field == null) {
                throw new ArgumentException("Field not found", nameof(field));
            }
        }
    }
}
