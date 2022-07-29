using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ToStringSourceGenerator.Extensions;

internal static class NamedTypeSymbolExtensions
{
    public static bool IsPartial(this INamedTypeSymbol type)
    {
        foreach (var declaration in type.DeclaringSyntaxReferences)
        {
            var syntax = declaration.GetSyntax();
            if (syntax is TypeDeclarationSyntax typeDeclaration)
            {
                if (typeDeclaration.Modifiers.Any(modifer => modifer.ValueText == "partial"))
                {
                    return true;
                }
            }
        }

        return false;
    }
}