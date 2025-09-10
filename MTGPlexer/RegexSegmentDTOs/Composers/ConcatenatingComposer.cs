namespace MTGPlexer.RegexSegmentDTOs.Composers;

public class ConcatenatingComposer : ISegmentComposer
{
    public static readonly ConcatenatingComposer Instance = new();
    private ConcatenatingComposer() { }

    public void Compose(RegexLineCollector collector, List<RegexSegmentBase> segments)
    {
        foreach (var segment in segments)
        {
            segment.ComposeRegexLines(collector);
        }
    }
}