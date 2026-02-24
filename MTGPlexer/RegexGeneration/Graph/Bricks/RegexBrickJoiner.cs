namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickJoiner : RegexBrick
{
    public Joiner Joiner { get; }

    public RegexBrickJoiner(RegexNode parentNode, Joiner joiner)
        : base(parentNode, joiner.GetDescription(), $"joiner {joiner}")
    {
        Joiner = joiner;
    }
}
