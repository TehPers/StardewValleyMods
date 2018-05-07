using System;

namespace TehCore.Helpers.Json {
    /// <summary>Indicates that a <see cref="T:TehCore.Configs.DescriptiveJsonConverter" /> should add descriptive comments based on <see cref="T:TehCore.Configs.DescriptionAttribute" />.</summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonDescribeAttribute : Attribute { }
}
