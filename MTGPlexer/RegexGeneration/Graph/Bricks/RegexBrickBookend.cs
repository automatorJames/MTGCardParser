namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickBookend : RegexBrick
{
    protected override int NestedDepthModifer => -1;

    public RegexBrickBookend(RegexNode parentNode, string regex, string comment)
        : base(parentNode, regex, comment)
    {
    }
}
