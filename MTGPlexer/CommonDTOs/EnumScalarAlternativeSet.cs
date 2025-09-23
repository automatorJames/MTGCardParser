namespace MTGPlexer.CommonDTOs;

public record EnumScalarAlternativeSet 
(
    List<EnumScalarAlternative> EnumAlternatives
) : ScalarAlternativeSet
    (
        Alternatives: EnumAlternatives.Select(x => x.RegexString).ToList()
    )
{
    public int ItemCount { get; } = EnumAlternatives.Count;
    public int LongestChildName { get; } = EnumAlternatives.Max(x => x.FriendlyName.Length);
}

