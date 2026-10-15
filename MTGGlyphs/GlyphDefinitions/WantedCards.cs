namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The cards a library search looks for: "a basic land card", "up to two forest cards", "a card named {this}", "a mercenary permanent card with mana value 3 or less".</summary>
/// <exampledoc>Borderland Ranger</exampledoc>
/// <examplecapture>a basic land card</examplecapture>
[Dependent]
public class WantedCards : Glyph
{
    public override Nib[] Nibs => [Opt("up to"), Prop(Quantity), Prop(Qualifiers), Prop(Kind), Plural("card"), Prop(Named), Prop(Limit)];

    public Quantity Quantity { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Optional]
    public PermanentKind Kind { get; set; }
    [Optional]
    public NamedThis Named { get; set; }
    [Optional]
    public ManaValueLimit Limit { get; set; }
}
