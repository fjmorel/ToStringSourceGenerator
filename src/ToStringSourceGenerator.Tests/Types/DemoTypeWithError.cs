﻿using ToStringSourceGenerator.Attributes;

namespace ToStringSourceGenerator.Tests.Types;

[AutoToString]
public partial class DemoTypeWithError
{
    public int Id { get; set; }
    public string? Text { get; set; }

    // Un comment and should give error
    //public override string ToString()
    //{
    //    return "Something";
    //}
}