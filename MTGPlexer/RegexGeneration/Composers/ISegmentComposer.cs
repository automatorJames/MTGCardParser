using MTGPlexer.RegexGeneration.RegexSegments;

namespace MTGPlexer.RegexGeneration.Composers;

public interface ISegmentComposer
{
    void Compose(RegexBuilder collector, List<RegexSegmentBase> segments);
}