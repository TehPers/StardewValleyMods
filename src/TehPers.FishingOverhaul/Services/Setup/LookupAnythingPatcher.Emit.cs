using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TehPers.FishingOverhaul.Services.Setup
{
    partial class LookupAnythingPatcher
    {
        private TypeInfo CreateTfoField()
        {
            var assemblyName = new AssemblyName($"{this.manifest.UniqueID}.Dynamic");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run
            );
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("LookupAnythingProxies");

            // Create class TfoFishSpawnRulesField
            var tb = moduleBuilder.DefineType(
                "TfoFishSpawnRulesField",
                TypeAttributes.Public | TypeAttributes.Class
            );

            // Create constructor
            LookupAnythingPatcher.CreateConstructor(tb);

            // Implement ICustomField
            LookupAnythingPatcher.ImplementCustomField(tb);

            return tb.CreateTypeInfo()!;
        }

        private static void CreateConstructor(TypeBuilder tb)
        {
            tb.DefineDefaultConstructor(
                MethodAttributes.Public
                | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName
            );
        }

        private static void ImplementCustomField(TypeBuilder tb)
        {
            tb.AddInterfaceImplementation(LookupAnythingPatcher.customFieldInterface.Value);

            // ICustomField.Label
            const MethodAttributes implPropAttrs = MethodAttributes.Public
                | MethodAttributes.Virtual
                | MethodAttributes.Final
                | MethodAttributes.SpecialName
                | MethodAttributes.HideBySig;
            LookupAnythingPatcher.DefineProperty(
                tb,
                "Label",
                typeof(string),
                accessorAttributes: implPropAttrs,
                getter: ilGenerator =>
                {
                    // return "TFO";
                    ilGenerator.Emit(OpCodes.Ldstr, "TFO");
                    ilGenerator.Emit(OpCodes.Ret);
                }
            );

            // ICustomField.Value
            var formattedTextInterface = AccessTools.TypeByName(
                "Pathoschild.Stardew.LookupAnything.Framework.IFormattedText"
            );
            LookupAnythingPatcher.DefineProperty(
                tb,
                "Value",
                formattedTextInterface.MakeArrayType(),
                accessorAttributes: implPropAttrs,
                getter: ilGenerator =>
                {
                    var arrayEmpty = AccessTools.Method(
                        typeof(Array),
                        nameof(Array.Empty),
                        generics: new[] { formattedTextInterface }
                    );

                    // return Array.Empty<IFormattedText>();
                    ilGenerator.Emit(OpCodes.Call, arrayEmpty);
                    ilGenerator.Emit(OpCodes.Ret);
                }
            );

            // ICustomField.HasValue
            LookupAnythingPatcher.DefineProperty(
                tb,
                "HasValue",
                typeof(bool),
                accessorAttributes: implPropAttrs,
                getter: ilGenerator =>
                {
                    // return true;
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    ilGenerator.Emit(OpCodes.Ret);
                }
            );

            // Vector2? ICustomField.DrawValue(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, float wrapWidth)
            LookupAnythingPatcher.DefineMethod(
                tb,
                "DrawValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final,
                typeof(Vector2?),
                new[] { typeof(SpriteBatch), typeof(SpriteFont), typeof(Vector2), typeof(float) },
                ilGenerator =>
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                    ilGenerator.Emit(OpCodes.Ret);
                }
            );
        }

        private static PropertyBuilder DefineProperty(
            TypeBuilder tb,
            string name,
            Type returnType,
            MethodAttributes accessorAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            Action<ILGenerator>? getter = null,
            Action<ILGenerator>? setter = null
        )
        {
            var propertyBuilder = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                returnType,
                null
            );

            // Define getter
            if (getter is not null)
            {
                var methodBuilder = LookupAnythingPatcher.DefineMethod(
                    tb,
                    $"get_{name}",
                    accessorAttributes,
                    returnType,
                    Type.EmptyTypes,
                    getter
                );
                propertyBuilder.SetGetMethod(methodBuilder);
            }

            // Define setter
            if (setter is not null)
            {
                var methodBuilder = LookupAnythingPatcher.DefineMethod(
                    tb,
                    $"set_{name}",
                    accessorAttributes,
                    null,
                    new[] { returnType },
                    setter
                );
                propertyBuilder.SetSetMethod(methodBuilder);
            }

            return propertyBuilder;
        }

        private static MethodBuilder DefineMethod(
            TypeBuilder tb,
            string name,
            MethodAttributes methodAttributes,
            Type? returnType,
            Type[] parameterTypes,
            Action<ILGenerator> body
        )
        {
            var methodBuilder = tb.DefineMethod(name, methodAttributes, returnType, parameterTypes);
            var ilGenerator = methodBuilder.GetILGenerator();
            body(ilGenerator);
            return methodBuilder;
        }
    }
}