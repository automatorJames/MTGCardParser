namespace MTGPlexer.CommonDTOs;

public record EnumScalarAlternateSet 
(
    List<EnumScalarAlternate> EnumAlternates
) : ScalarAlternateSet
    (
        Alternates: EnumAlternates.Select(x => x.RegexString).ToList()
    )
{
    public int ItemCount { get; } = EnumAlternates.Count;
    public int LongestChildName { get; } = EnumAlternates.Max(x => x.DisplayName.Length);
}

