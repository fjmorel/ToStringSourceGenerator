using Microsoft.CodeAnalysis;

namespace ToStringSourceGenerator;

public enum ObjectSeparatorToken
{
    None,
    Brace,
    Array,
    Quote,
}

public static class ObjectSeparatorTokensExtensions
{
    public static string GetOpeningSeparatorFor(this ObjectSeparatorToken separator) => separator switch
    {
        ObjectSeparatorToken.None => "",
        ObjectSeparatorToken.Brace => "{{ ",
        ObjectSeparatorToken.Array => "[ ",
        ObjectSeparatorToken.Quote => "\\\"",
        _ => throw new ArgumentException($"Unexpected separator value: '{separator}'"),
    };

    public static string GetClosingSeparatorFor(this ObjectSeparatorToken separator) => separator switch
    {
        ObjectSeparatorToken.None => "",
        ObjectSeparatorToken.Brace => " }}",
        ObjectSeparatorToken.Array => " ]",
        ObjectSeparatorToken.Quote => "\\\"",
        _ => throw new ArgumentException($"Unexpected separator value: '{separator}'"),
    };

    public static ObjectSeparatorToken GetSeparatorFor(SpecialType specialType)
    {
        switch (specialType)
        {
            case SpecialType.None:
            case SpecialType.System_Object:
                return ObjectSeparatorToken.Brace;
            case SpecialType.System_Array:
            case SpecialType.System_Collections_IEnumerable:
            case SpecialType.System_Collections_Generic_IEnumerable_T:
            case SpecialType.System_Collections_Generic_IList_T:
            case SpecialType.System_Collections_Generic_ICollection_T:
            case SpecialType.System_Collections_IEnumerator:
            case SpecialType.System_Collections_Generic_IEnumerator_T:
            case SpecialType.System_Collections_Generic_IReadOnlyList_T:
            case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                return ObjectSeparatorToken.Array;
            case SpecialType.System_String:
            case SpecialType.System_DateTime:
                return ObjectSeparatorToken.Quote;
            default:
                return ObjectSeparatorToken.None;
        }
    }
}