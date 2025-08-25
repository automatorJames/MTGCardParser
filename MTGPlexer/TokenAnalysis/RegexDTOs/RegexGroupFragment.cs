namespace MTGPlexer.TokenAnalysis.RegexDTOs.Internal;

/// <summary>
/// Represents a container like (...), [...], or a named group.
/// </summary>
public record RegexGroupFragment(
    RegexGroupType Type,
    string OpeningDelimiter,
    string ClosingDelimiter,
    List<RegexFragment> Children,
    string Name = null,
    string Comment = null,
    string Quantifier = null
) : RegexFragment
{
    public override string ToString()
    {
        var str = OpeningDelimiter;

        foreach (var child in Children)
            str += child.ToString();

        str += ClosingDelimiter;

        return str;
    }
}

public enum RegexGroupType
{
    Root,
    NamedCapture,
    AnonymousCapture,
    TokenUnitOneOf,
    CharacterClass
}