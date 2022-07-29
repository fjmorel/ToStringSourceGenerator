namespace ToStringSourceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class AutoToStringAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class FormatToStringAttribute : Attribute
{
    public FormatToStringAttribute(string format)
    {
        Format = format;
    }

    public string Format { get; }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class SkipToStringAttribute : Attribute { }