﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ToStringSourceGenerator.Utils;

/// <summary>
/// Utility class extracted from https://github.com/terrajobst/minsk/blob/master/src/Minsk.Generators/SyntaxNodeGetChildrenGenerator.cs
/// https://youtu.be/JSFZ3qDx83g?list=PLRAdsfhKI4OWNOSfS7EUu5GRAVmze1t2y&t=1524
/// </summary>
internal static class CompilationHelper
{
    public static IReadOnlyList<INamedTypeSymbol> GetAllTypes(IAssemblySymbol symbol)
    {
        var result = new List<INamedTypeSymbol>();
        GetAllTypes(result, symbol.GlobalNamespace);
        result.Sort((x, y) => string.Compare(x.MetadataName, y.MetadataName, StringComparison.Ordinal));
        return result;
    }

    public static void GetAllTypes(List<INamedTypeSymbol> result, INamespaceOrTypeSymbol symbol)
    {
        if (symbol is INamedTypeSymbol type)
            result.Add(type);

        foreach (var child in symbol.GetMembers())
            if (child is INamespaceOrTypeSymbol nsChild)
                GetAllTypes(result, nsChild);
    }

    public static bool IsDerivedFrom(ITypeSymbol type, INamedTypeSymbol baseType)
    {
        var iteratorType = type;
        while (iteratorType != null)
        {
            if (SymbolEqualityComparer.Default.Equals(type, baseType))
                return true;

            iteratorType = iteratorType.BaseType;
        }

        return false;
    }

    public static bool IsPartial(INamedTypeSymbol type)
    {
        foreach (var declaration in type.DeclaringSyntaxReferences)
        {
            var syntax = declaration.GetSyntax();
            if (syntax is TypeDeclarationSyntax typeDeclaration)
            {
                foreach (var modifer in typeDeclaration.Modifiers)
                {
                    if (modifer.ValueText == "partial")
                        return true;
                }
            }
        }

        return false;
    }

    public static bool SymbolContainsAttribute(ISymbol symbol, INamedTypeSymbol attributeSymbol)
    {
        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default))
                return true;
        }

        return false;
    }

    public static IEnumerable<AttributeData> GetAttributesOfType(ISymbol symbol, INamedTypeSymbol attributeSymbol)
    {
        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default))
                yield return attr;
        }
    }
}