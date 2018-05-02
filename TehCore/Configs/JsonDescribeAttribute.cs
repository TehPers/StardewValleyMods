using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehCore.Configs {
    /// <summary>Indicates that a <see cref="T:TehCore.Configs.DescriptiveJsonConverter" /> should add descriptive comments based on <see cref="T:TehCore.Configs.DescriptionAttribute" />.</summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonDescribeAttribute : Attribute { }
}
