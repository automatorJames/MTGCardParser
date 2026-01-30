namespace MTGPlexer.RegexGeneration.Composers;

public class ConcatenatingComposer : ISegmentComposer
{
    public static readonly ConcatenatingComposer Instance = new();
    private ConcatenatingComposer() { }

    public void Compose(RegexBuilder collector, List<Node> nodes)
    {
        foreach (var node in nodes)
            node.ComposeRegexLines(collector);
    }

}