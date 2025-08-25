/*namespace MTGPlexer.TokenAnalysis.RegexDTOs;

using System.Collections.Generic;

// Base interface for the Composite Pattern.
public interface IRegexFragment
{
    IRegexFragment Parent { get; set; }
}

// Represents a container like (...), [...], or a named group.
public class RegexGroupFragment : IRegexFragment
{
    public IRegexFragment Parent { get; set; }
    public List<IRegexFragment> Children { get; } = [];
    public RegexGroupType Type { get; }
    public string OpeningDelimiter { get; }
    public string ClosingDelimiter { get; }
    public string Name { get; init; } // For named capture groups
    public string Comment { get; init; } // For (?#...) groups
    public string Quantifier { get; set; }

    public RegexGroupFragment(IRegexFragment parent, RegexGroupType type, string open, string close)
    {
        Parent = parent;
        Type = type;
        OpeningDelimiter = open;
        ClosingDelimiter = close;
    }
}

// Represents a "leaf" - a piece of literal text, an escaped character, or a separator.
public class RegexTextFragment : IRegexFragment
{
    public IRegexFragment Parent { get; set; }
    public string Text { get; }
    public RegexTextFragment(IRegexFragment parent, string text)
    {
        Parent = parent;
        Text = text;
    }
}

public enum RegexGroupType
{
    Root,
    NamedCapture,
    AnonymousCapture,
    TokenUnitOneOf,
    CharacterClass,
    QuantifierBraces // For future use, e.g., {1,3}
}*/