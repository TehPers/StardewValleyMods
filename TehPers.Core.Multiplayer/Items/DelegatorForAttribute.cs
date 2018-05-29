using System;
using System.Reflection;
using System.Security.Policy;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Multiplayer.Items {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class DelegatorForAttribute : Attribute {
        public Type TargetType { get; }
        public MethodInfo TargetMethod { get; }

        public DelegatorForAttribute(string typeName, string methodName) {
            this.TargetType = AssortedHelpers.GetSDVType(typeName);
            this.TargetMethod = this.TargetType?.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public DelegatorForAttribute(Type targetType, MethodInfo targetMethod) {
            this.TargetType = targetType;
            this.TargetMethod = targetMethod;
        }
    }
}