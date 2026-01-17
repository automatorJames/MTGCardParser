namespace MTGPlexer.CommonDTOs;

public record ExtractedCapture
{
    public int Index { get; }
    public int Length { get; }
    public int End { get; }
    public string Value { get; }

    public ExtractedCapture(Capture capture)
    {
        Index = capture.Index;
        Length = capture.Length;
        End = Index + Length;
        Value = capture.Value;
    }
}
