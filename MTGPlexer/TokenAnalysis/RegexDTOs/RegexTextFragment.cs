namespace MTGPlexer.TokenAnalysis.RegexDTOs.Internal;

/// <summary>
/// Represents a "leaf" - a piece of literal text, an escaped character, or a separator.
/// </summary>
public record RegexTextFragment(string Text) : RegexFragment
{
    public override string ToString() => Text;
}