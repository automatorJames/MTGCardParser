namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>Base type for the opening/closing bricks that bookend a named group, e.g. <c>(?&lt;name&gt;</c> and <c>)</c>.</summary>
public class RegexBrickGroupBookend : RegexBrick
{
    protected override int NestedDepthModifer => -1;

    public RegexBrickGroupBookend(RegexNode parentNode, string regex)
        : base(parentNode, regex)
    {
    }
}
