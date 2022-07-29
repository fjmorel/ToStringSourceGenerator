using Microsoft.CodeAnalysis;

namespace ToStringSourceGenerator.Extensions;

/// <summary>
/// Utility class extracted from https://github.com/terrajobst/minsk/blob/master/src/Minsk.Generators/SyntaxNodeGetChildrenGenerator.cs
/// Some of these helpers have moved elsewhere now.
/// </summary>
internal static class SymbolExtensions
{
    public static bool ContainsAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
        => symbol.GetAttributes().Any(x => attributeSymbol.Equals(x.AttributeClass, SymbolEqualityComparer.IncludeNullability));

    public static IEnumerable<AttributeData> GetAttributesOfType(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
        => symbol.GetAttributes().Where(x => attributeSymbol.Equals(x.AttributeClass, SymbolEqualityComparer.IncludeNullability));
}