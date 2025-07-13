namespace MTGPlexer.RegexSegmentDTOs;

public record class ScalarPropVal(RegexPropInfo RegexPropInfo, object Val, int Position, string ParentPath)
{
    public RegexPropInfo Prop { get; } = RegexPropInfo;
    public string Name { get; } = RegexPropInfo.Name.ToFriendlyCase(Extensions.TitleDisplayOption.Sentence);
    public string Path { get; } = ParentPath + "." + RegexPropInfo.Name;
    public string ValueAsString { get; } = Val.ToString();
    public DeterministicPalette Palette { get; } = new(Position);
}

