namespace MTGPlexer.RegexGeneration.Graph;

public class RegexBrickBookendRoot : RegexBrickBookend
{
    public RegexBrickBookendRoot(RegexNode parentNode, string regex)
        : base(parentNode, regex, "")
    {
        Debugger.Break();
    }

    protected override int CalculateNestedDepth() =>
        base.CalculateNestedDepth() - 1;
}
