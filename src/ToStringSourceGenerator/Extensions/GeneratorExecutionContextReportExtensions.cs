using Microsoft.CodeAnalysis;
using ToStringSourceGenerator.Attributes;

namespace ToStringSourceGenerator.Extensions;

/// <summary>
/// Extension methods for reporting various diagnostics on user's source code
/// </summary>
internal static class GeneratorExecutionContextReportExtensions
{
    public static void ReportMultiplesFormatStringAttributesApplied(this GeneratorExecutionContext context, INamedTypeSymbol type)
    {
        // TODO Reportar mejor la localizacion
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                nameof(FormatToStringAttribute),
                nameof(FormatToStringAttribute),
                $"Multiples attributes form No properties found in '{type.ContainingNamespace}.{type.Name}' type to fill ToString() method.",
                $"{nameof(AutoToStringAttribute)}",
                DiagnosticSeverity.Warning,
                true),
            type.Locations.FirstOrDefault() ?? Location.None
        ));
    }

    public static void ReportNoPropertiesFoundOnMethod(this GeneratorExecutionContext context, INamedTypeSymbol type)
    {
        // TODO Reportar mejor la localizacion
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                nameof(AutoToStringAttribute),
                nameof(AutoToStringAttribute),
                $"No properties found in '{type.ContainingNamespace}.{type.Name}' type to fill ToString() method.",
                $"{nameof(AutoToStringAttribute)}",
                DiagnosticSeverity.Warning,
                true),
            type.Locations.FirstOrDefault() ?? Location.None
        ));
    }

    public static void ReportClassMustBePartial(this GeneratorExecutionContext context, INamedTypeSymbol type)
    {
        // TODO Reportar mejor la localizacion
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                nameof(AutoToStringAttribute),
                nameof(AutoToStringAttribute),
                $"'{type.ContainingNamespace}.{type.Name}' class must be partial, if '{typeof(AutoToStringAttribute).FullName}' is used",
                $"{nameof(AutoToStringAttribute)}",
                DiagnosticSeverity.Warning,
                true),
            type.Locations.FirstOrDefault() ?? Location.None
        ));
    }

    public static void ReportClassContainsToStringWithNoArguments(this GeneratorExecutionContext context, INamedTypeSymbol type, IMethodSymbol? method)
    {
        // TODO Reportar mejor la localizacion
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                nameof(AutoToStringAttribute),
                nameof(AutoToStringAttribute),
                $"Method 'ToString()' can not be overriden, in type '{type.ContainingNamespace}.{type.Name}' if has attribute '{typeof(AutoToStringAttribute).FullName}'",
                $"{nameof(AutoToStringAttribute)}",
                DiagnosticSeverity.Error,
                true),
            method?.Locations.FirstOrDefault() ?? Location.None
        ));
    }
}