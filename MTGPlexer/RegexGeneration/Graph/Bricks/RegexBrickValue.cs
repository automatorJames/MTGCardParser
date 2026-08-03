namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>A brick that matches one specific scalar value, e.g. a single enum member's regex pattern.</summary>
public class RegexBrickValue : RegexBrick
{
    /// <summary>The CLR value (e.g. enum member) this brick's pattern matches.</summary>
    public object Value { get; }

    public RegexBrickValue(RegexNode parentNode, string regex, object value)
        : base(parentNode, regex)
    {
        Value = value;
    }
}