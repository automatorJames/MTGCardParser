using MTGPlexer.RegexGeneration.RegexSegments;

namespace MTGPlexer.RegexGeneration.Composers;

public interface ISegmentComposer
{
    void Compose(RegexLineCollector collector, List<RegexSegmentBase> segments);
}