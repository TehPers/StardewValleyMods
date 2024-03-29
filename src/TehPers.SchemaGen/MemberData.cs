﻿using Namotion.Reflection;

namespace TehPers.SchemaGen
{
    internal record MemberData(
        ContextualAccessorInfo Accessor,
        bool IsStatic,
        bool IsFullyPublic
    )
    {
        public MemberData(ContextualFieldInfo field)
            : this(field, field.FieldInfo.IsStatic, field.FieldInfo.IsPublic)
        {
        }

        public MemberData(ContextualPropertyInfo property)
            : this(
                property,
                property.PropertyInfo.GetMethod?.IsStatic is true
                || property.PropertyInfo.SetMethod?.IsStatic is true,
                property.PropertyInfo.GetMethod?.IsPublic is not false
                && property.PropertyInfo.SetMethod?.IsPublic is not false
            )
        {
        }
    }
}
