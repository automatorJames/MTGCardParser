namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The cards a library search looks for: "a basic land card", "up to two forest cards", "a mountain or plains card", "a blue or black creature card", "a card named {this}", "a card named ajani, valiant protector", "a mercenary permanent card with mana value 3 or less", "a creature card with mana value 6 or greater", "a creature card with flying", "a card with the same mana value as this card", "any number of cards with that name", "up to two basic land cards and/or desert cards", "a basic land card or gate card".</summary>
/// <exampledoc>Borderland Ranger</exampledoc>
/// <examplecapture>a basic land card</examplecapture>
[Dependent]
public class WantedCards : Glyph
{
    public override Nib[] Nibs => [Opt("up to"), Prop(Quantity), Prop(Qualifiers), Prop(Kind), Plural("card"), Prop(OrKind), Prop(Named), Prop(Limit)];

    public Quantity Quantity { get; set; }
    [Optional]
    public QualifierWords Qualifiers { get; set; }
    [Optional]
    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
    [Optional]
    public AlternativeWantedKind OrKind { get; set; }
    [Optional]
    public OneOf<NamedThis, NamedCard> Named { get; set; }
    [Optional]
    public OneOf<ManaValueLimit, SameCharacteristicAs, OneOf<WithName, WithQualifier>> Limit { get; set; }
}
