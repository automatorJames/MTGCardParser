namespace MTGPlexer.RegexGeneration.Graph;

public class RegexBrickAlternatingPipe : RegexBrick
{
    public RegexBrickAlternatingPipe(RegexNode parentNode)
        : base(parentNode, "|", "joiner pipe")
    {
    }
}
