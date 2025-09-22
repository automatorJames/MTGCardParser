namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record Enclosure
{
    public int Ordinal { get; }
    public EnclosureType Type { get; }
    public GroupBorderTreatment Treatment { get; }

    public Enclosure(int ordinal, EnclosureType type, GroupBorderTreatment treatment)
    {
        Ordinal = ordinal;
        Type = type;
        Treatment = treatment;
    }

    public Enclosure(int ordinal)
    {
        Ordinal = ordinal;
        Type = EnclosureType.Unnamed;
        Treatment = GroupBorderTreatment.Brace;
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

