namespace MTGPlexer.RegexSegmentDTOs;

public record class ScalarPropVal(RegexPropInfo RegexPropInfo, object Val, int Position, string ParentPath)
{
    public string Path { get; } = ParentPath + "." + RegexPropInfo.Name;
    public string StringVal { get; } = Val.ToString();
    public string HexColor { get; } = TokenClassRegistry.PropertyCaptureColors[Position];
}

