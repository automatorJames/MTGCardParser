namespace MTGPlexer.TokenAnalysis.RegexDTOs.Internal;

using System.Collections.Generic;
using System.Text;

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
        var sb = new StringBuilder();
        sb.Append(OpeningDelimiter);
        foreach (var child in Children)
        {
            sb.Append(child.ToString());
        }
        sb.Append(ClosingDelimiter);
        sb.Append(Quantifier);
        return sb.ToString();
    }
}

public enum RegexGroupType
{
    Root,
    NamedCapture,
    AnonymousCapture,
    Comment,
    CharacterClass
}