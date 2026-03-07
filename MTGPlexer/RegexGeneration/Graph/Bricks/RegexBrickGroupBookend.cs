namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickGroupBookend : RegexBrick
{
    protected override int NestedDepthModifer => -1;

    public RegexBrickGroupBookend(RegexNode parentNode, string regex, string comment)
        : base(parentNode, regex, comment)
    {
    }
}
