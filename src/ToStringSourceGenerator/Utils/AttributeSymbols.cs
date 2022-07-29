using Microsoft.CodeAnalysis;

namespace ToStringSourceGenerator.Utils;

public class AttributeSymbols
{
    public INamedTypeSymbol Auto { get; }
    public INamedTypeSymbol Format { get; }
    public INamedTypeSymbol Skip { get; }

    public AttributeSymbols(
        INamedTypeSymbol auto,
        INamedTypeSymbol format,
        INamedTypeSymbol skip
    )
    {
        Auto = auto;
        Format = format;
        Skip = skip;
    }
}