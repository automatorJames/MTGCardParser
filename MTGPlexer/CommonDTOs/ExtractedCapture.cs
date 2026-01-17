namespace MTGPlexer.CommonDTOs;

public record ExtractedCapture
{
    public string Name  { get; }
    public int Index { get; }
    public int Length { get; }
    public int End { get; }
    public string Value { get; }
    public int Ordinal { get; }
    public int SiblingBranchCount { get; }

    public ExtractedCapture(Capture capture, string name, int ordinal = 0, int siblingBranchCount = 0)
    {
        Index = capture.Index;
        Length = capture.Length;
        End = Index + Length;
        Value = capture.Value;
        Name = name;
        Ordinal = ordinal;
        SiblingBranchCount = siblingBranchCount;
    }
}
