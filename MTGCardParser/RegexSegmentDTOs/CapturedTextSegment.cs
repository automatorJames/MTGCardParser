namespace MTGCardParser.RegexSegmentDTOs;

public record CapturedTextSegment
(
    string Text
)
{
    public override string ToString() => Text;
}

