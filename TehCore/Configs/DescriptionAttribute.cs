using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehCore.Configs {

    /// <summary>Describes a field or property.</summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute {
        public string Description { get; }

        /// <param name="description">A custom description for the property/field</param>
        /// <inheritdoc />
        public DescriptionAttribute(string description) {
            this.Description = description;
        }
    }
}
