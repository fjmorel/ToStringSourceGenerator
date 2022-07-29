using ToStringSourceGenerator.Attributes;

namespace ToStringSourceGenerator.Tests.Types;

[AutoToString]
public partial class DemoType
{
    public int Id { get; set; }
    public string? Text { get; set; }

    [SkipToString]
    public string? Password { get; set; }

    [FormatToString("HH:mm")]
    public DateTime Time { get; set; }

    private string? PrivateValue { get; set; }
}