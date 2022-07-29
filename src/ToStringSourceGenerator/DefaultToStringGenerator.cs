using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Text;
using ToStringSourceGenerator.Extensions;

namespace ToStringSourceGenerator;

public sealed class DefaultToStringGenerator
{
    private const char _valuesSeparator = ',';
    private const char _propertySeparator = ':';

    private readonly GeneratorExecutionContext _context;
    private readonly AttributeSymbols _attributes;

    public DefaultToStringGenerator(GeneratorExecutionContext context, AttributeSymbols attributes)
    {
        _context = context;
        _attributes = attributes;
    }

    internal void WriteType(INamedTypeSymbol type, IndentedTextWriter indentedTextWriter)
    {
        if (!type.IsPartial())
        {
            _context.ReportClassMustBePartial(type);
        }
        else if (ContainsToStringMethodWithNoArguments(type))
        {
            _context.ReportClassContainsToStringWithNoArguments(type, GetToStringMethodWithNoArguments(type));
        }
        else if (!GetSymbolsForToString(type, _attributes.Skip).Any())
        {
            _context.ReportNoPropertiesFoundOnMethod(type);
        }
        else
        {
            WritePartialClassSourceTextTo(type, indentedTextWriter);
        }
    }

    private void WritePartialClassSourceTextTo(INamedTypeSymbol type, IndentedTextWriter indentedTextWriter)
    {
        // TODO Comprobar si contiene to string

        indentedTextWriter.WriteLine();
        indentedTextWriter.WriteLine($"namespace {type.ContainingNamespace}");
        indentedTextWriter.WriteLine("{");
        indentedTextWriter.Indent++;
        indentedTextWriter.WriteLine("using System;");
        indentedTextWriter.WriteLine();
        indentedTextWriter.WriteLine($"partial class {type.Name}");
        indentedTextWriter.WriteLine("{");
        indentedTextWriter.Indent++;

        indentedTextWriter.WriteLine("public override string ToString()");
        indentedTextWriter.WriteLine("{");
        indentedTextWriter.Indent++;

        WriteToStringMethodBody(type, indentedTextWriter);
        indentedTextWriter.Indent--;
        indentedTextWriter.WriteLine("}");

        indentedTextWriter.Indent--;
        indentedTextWriter.WriteLine("}");

        indentedTextWriter.Indent--;
        indentedTextWriter.WriteLine("}");
    }

    private void WriteToStringMethodBody(INamedTypeSymbol type, IndentedTextWriter indentedTextWriter)
    {
        indentedTextWriter.Write("return $\"");
        var stringValueInMethod = new StringBuilder();

        var count = 0;
        foreach (var propertySymbol in GetSymbolsForToString(type, _attributes.Skip))
        {
            if (count > 0)
                stringValueInMethod.Append(' ');
            stringValueInMethod.Append(propertySymbol.Name);
            stringValueInMethod.Append(_propertySeparator);
            stringValueInMethod.Append(' ');// Add space

            WritePropertyValueToStringRepresentation(stringValueInMethod, propertySymbol, _attributes.Format);

            stringValueInMethod.Append(_valuesSeparator);// Add value separator

            count++;
        }

        // remove trailing separator
        stringValueInMethod.Length -= 1;
        indentedTextWriter.Write(stringValueInMethod.ToString());
        indentedTextWriter.Write("\";");
    }

    private static void WritePropertyValueToStringRepresentation(StringBuilder sb, IPropertySymbol namedTypeSymbol, INamedTypeSymbol formatAttributeSymbol)
    {
        var propertyValueEnclosingDelimiter = ObjectSeparatorTokensExtensions.GetSeparatorFor(namedTypeSymbol.Type.SpecialType);
        sb.Append(propertyValueEnclosingDelimiter.GetOpeningSeparatorFor());

        var attributeFormatString = namedTypeSymbol.GetAttributesOfType(formatAttributeSymbol).SingleOrDefault();
        if (attributeFormatString != null)
        {
            var format = GetFirstConstructorArgumentValueOfAttribute(attributeFormatString);
            sb.Append($"{{{namedTypeSymbol.Name}:{format}}}");
        }
        else
        {
            sb.Append($"{{{namedTypeSymbol.Name}}}");
        }

        sb.Append(propertyValueEnclosingDelimiter.GetClosingSeparatorFor());
    }


    private static bool ContainsToStringMethodWithNoArguments(ITypeSymbol type)
    {
        return GetToStringMethodWithNoArguments(type) != null;
    }

    private static IEnumerable<IPropertySymbol> GetSymbolsForToString(INamedTypeSymbol type, INamedTypeSymbol skipAttributeSymbol)
    {
        foreach (var typeProperty in type.GetMembers().Where(t => t.Kind == SymbolKind.Property))
        {
            if (typeProperty is IPropertySymbol propertySymbol)
            {
                var visible = propertySymbol.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal;
                var containsSkipAttribute = propertySymbol.ContainsAttribute(skipAttributeSymbol);

                if (visible && !containsSkipAttribute)
                {
                    yield return propertySymbol;
                }
            }
        }
    }

    private static IMethodSymbol? GetToStringMethodWithNoArguments(ITypeSymbol type)
    {
        var toStringMembers = type.GetMembers("ToString");
        foreach (var toStringMember in toStringMembers)
        {
            if (toStringMember is IMethodSymbol toStringMethodSymbol)
            {
                if (toStringMethodSymbol.Parameters.Length == 0)
                    return toStringMethodSymbol;
            }
        }

        return null;
    }

    private static string? GetFirstConstructorArgumentValueOfAttribute(AttributeData data)
    {
        var constructorArgument = data.ConstructorArguments.First();
        return (string?)constructorArgument.Value;
    }
}