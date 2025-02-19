﻿using ToStringSourceGenerator.Attributes;

namespace ToStringSourceGenerator.Tests.Types;

[AutoToString]
public partial class DemoNestedType
{
    public DemoNestedType()
    {
        Prop = new DemoType();
    }

    public int Id { get; set; }
    public string? Text { get; set; }
    public DemoType Prop { get; set; }
}