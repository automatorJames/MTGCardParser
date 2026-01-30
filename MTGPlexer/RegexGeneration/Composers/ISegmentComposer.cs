namespace MTGPlexer.RegexGeneration.Composers;

public interface ISegmentComposer
{
    void Compose(RegexBuilder collector, List<Node> nodes);
}