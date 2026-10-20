namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The cards a library search looks for: "a basic land card", "up to two forest cards", "a mountain or plains card", "a card named {this}", "a mercenary permanent card with mana value 3 or less", "a card with the same mana value as this card".</summary>
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
    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
    [Optional]
    public NamedThis Named { get; set; }
    [Optional]
    public OneOf<ManaValueLimit, SameCharacteristicAs> Limit { get; set; }
}
