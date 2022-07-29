using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Text;
using ToStringSourceGenerator.Generators;
using ToStringSourceGenerator.Utils;

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
        context.AddCompiledOnMetadataAttribute();

        var compilation = context.Compilation;
        var types = CompilationHelper.GetAllTypes(context.Compilation.Assembly);
        
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
            if (type is not null && defaultToStringGenerator.ShouldUseGenerator(type))
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
}