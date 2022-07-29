using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Text;
using ToStringSourceGenerator.Extensions;

namespace ToStringSourceGenerator;

[Generator]
public class SourceGeneratorToString : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var compiledOnText = SourceText.From($"[assembly: System.Reflection.AssemblyMetadata(\"CompiledOn:\", \"{DateTime.UtcNow}\")]", Encoding.UTF8);
        context.AddSource("Generated.cs", compiledOnText);

        var compilation = context.Compilation;
        var types = GetAllTypes(context.Compilation.Assembly);

        var attributes = new AttributeSymbols(
            context.Compilation.GetTypeByMetadataName("ToStringSourceGenerator.Attributes.AutoToStringAttribute")!,
            context.Compilation.GetTypeByMetadataName("ToStringSourceGenerator.Attributes.FormatToStringAttribute")!,
            context.Compilation.GetTypeByMetadataName("ToStringSourceGenerator.Attributes.SkipToStringAttribute")!
        );

        using var stringWriter = new StringWriter();
        using var indentedTextWriter = new IndentedTextWriter(stringWriter, "    ");
        var defaultToStringGenerator = new DefaultToStringGenerator(context, attributes);
        foreach (var type in types)
        {
            if (type is not null && type.ContainsAttribute(attributes.Auto))
            {
                defaultToStringGenerator.WriteType(type, indentedTextWriter);
            }
        }

        indentedTextWriter.Flush();
        stringWriter.Flush();

        var sourceText = SourceText.From(stringWriter.ToString(), Encoding.UTF8);
        var hintName = $"AutoToString_{compilation.Assembly.Name}.g.cs";

        context.AddSource(hintName, sourceText);
    }

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(IAssemblySymbol symbol)
    {
        var result = new List<INamedTypeSymbol>();
        GetAllTypes(result, symbol.GlobalNamespace);
        return result.OrderBy(x => x.MetadataName, StringComparer.Ordinal);
    }

    private static void GetAllTypes(List<INamedTypeSymbol> result, INamespaceOrTypeSymbol symbol)
    {
        if (symbol is INamedTypeSymbol type)
            result.Add(type);

        foreach (var child in symbol.GetMembers())
            if (child is INamespaceOrTypeSymbol nsChild)
                GetAllTypes(result, nsChild);
    }
}