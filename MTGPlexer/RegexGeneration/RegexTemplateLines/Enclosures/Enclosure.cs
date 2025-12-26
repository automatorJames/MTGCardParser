namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record Enclosure
{
    public int Ordinal { get; }
    public HexPalette Palette { get; }
    public EnclosureType Type { get; }
    public GroupBorderTreatment Treatment { get; }

    public Enclosure(int ordinal, HexPalette palette, EnclosureType type, GroupBorderTreatment treatment)
    {
        Ordinal = ordinal;
        Palette = palette;
        Type = type;
        Treatment = treatment;
    }

    public Enclosure(int ordinal)
    {
        Ordinal = ordinal;
        Palette = DeterministicPalette.GetStaticPalette(new HexColor("#696969"));
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

