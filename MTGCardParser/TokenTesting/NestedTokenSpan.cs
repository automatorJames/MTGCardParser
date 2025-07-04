public record NestedTokenSpan
{
    public Type TokenUnitType { get; init; }
    public TextSpan TextSpan { get; init; }
    public int Index { get; init; }
    public int Length { get; init; }
    public List<ITokenUnit> TokenChildren { get; init; }
    public List<NestedTokenSpan> SpanChildren { get; private set; }

    public NestedTokenSpan(Type tokenUnitType, TextSpan textSpan, int index, int length, List<ITokenUnit> tokenChildren)
    {
        var flattenedChildren = token

        TokenUnitType = tokenUnitType;
        TextSpan = textSpan;
        Index = index;
        Length = length;
        TokenChildren = tokenChildren;
    }
}

