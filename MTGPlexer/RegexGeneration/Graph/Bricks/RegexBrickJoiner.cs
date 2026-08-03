namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>The regex text (if any) that joins two sibling bricks together, e.g. a literal pipe between alternatives.</summary>
public class RegexBrickJoiner : RegexBrick
{
    /// <summary>Which joining rule produced this brick's regex text.</summary>
    public Joiner Joiner { get; }

    public RegexBrickJoiner(RegexNode parentNode, Joiner joiner)
        : base(parentNode, joiner.GetDescription())
    {
        Joiner = joiner;
    }
}
