namespace MTGPlexer.RegexGeneration.Graph;

public class RegexBrickJoiner : RegexBrick
{
    public Joiner Joiner { get; }

    public RegexBrickJoiner(RegexNode parentNode, Joiner joiner)
        : base(parentNode, joiner.GetDescription(), $"joiner {joiner}")
    {
        Joiner = joiner;
    }
}
