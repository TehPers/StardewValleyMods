using System;
using System.ComponentModel;

namespace TehPers.Core.Api.Json
{
    /// <summary>Indicates that the properties of this type that are annotated with <see cref="DescriptionAttribute"/> should be commented when serialized by an <see cref="ICommentedJsonApi"/>.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonDescribeAttribute : Attribute
    {
    }
}
