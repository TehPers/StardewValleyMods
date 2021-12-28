using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TehPers.Core.SourceGen
{
    internal class ConstructFromTypeConversion
    {
        public TypeDeclarationSyntax TypeDeclaration { get; }

        public TypeSyntax FromType { get; }

        public ConstructFromTypeConversion(TypeDeclarationSyntax typeDeclaration, TypeSyntax fromType)
        {
            this.TypeDeclaration = typeDeclaration;
            this.FromType = fromType;
        }
    }
}
