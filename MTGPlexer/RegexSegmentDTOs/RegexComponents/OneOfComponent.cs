namespace MTGPlexer.RegexSegmentDTOs.RegexComponents;

public class OneOfComponent : RegexComponentBase
{
    public List<RegexComponentBase> BaseItems { get; set; } = [];
}

