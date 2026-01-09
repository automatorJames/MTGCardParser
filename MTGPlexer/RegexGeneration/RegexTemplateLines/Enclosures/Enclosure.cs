namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record Enclosure
{
    public int Ordinal { get; }
    public int Depth { get; }
    public EnclosureType Type { get; }
    public GroupBorderTreatment Treatment { get; }
    public SpaceDisposition SpaceDisposition { get; }

    public Enclosure(
        int ordinal, 
        int depth, 
        EnclosureType type = EnclosureType.Unnamed,
        GroupBorderTreatment treatment  = GroupBorderTreatment.Brace,
        SpaceDisposition? spaceDisposition = null)
    {
        Ordinal = ordinal;
        Depth = depth;
        Type = type;
        Treatment = treatment;
        SpaceDisposition = spaceDisposition ?? SpaceDisposition.Default;
    }
}

public enum EnclosureType
{
    Root,
    RegexProp,
    NameOverride,
    Unnamed
}

public enum GroupBorderTreatment
{
    None,       //

    ClosedBox,  // ─ │ ┌ ┐ └ ┘

    DashedBox,  // ╌ ╎ ┌ ┐ └ ┘

    Brace,      //   ┊ ╭ ╮ ╰ ╯
}

