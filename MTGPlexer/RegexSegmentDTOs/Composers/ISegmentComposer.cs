namespace MTGPlexer.RegexSegmentDTOs.Composers;

public interface ISegmentComposer
{
    void Compose(RegexLineCollector collector, List<RegexSegmentBase> segments);
}