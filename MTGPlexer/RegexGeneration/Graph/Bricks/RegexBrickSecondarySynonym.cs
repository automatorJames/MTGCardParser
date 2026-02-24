namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickSecondarySynonym : RegexBrick
{
    public RegexBrickSecondarySynonym(RegexNode parentNode, string regex, string comment)
        : base(parentNode, regex, comment)
    {
    }

    protected override int CalculateNestedDepth() =>
        base.CalculateNestedDepth() + 1;

    public override string ToString() => Regex;
}
