namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickBookend : RegexBrick
{
    public RegexBrickBookend(RegexNode parentNode, string regex, string comment)
        : base(parentNode, regex, comment)
    {
    }

    protected override int CalculateNestedDepth() =>
        base.CalculateNestedDepth() - 1;
}
