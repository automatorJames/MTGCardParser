namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards picked out of a group just revealed, looked at or exiled: "a creature or land card from among them", "up to two creature cards with mana value 3 or less from among them", "all land cards from among them", "a nonland permanent card with mana value x or less from among them", "any number of creature and/or land cards from among them".</summary>
/// <exampledoc>Grisly Salvage</exampledoc>
/// <examplecapture>a creature or land card from among them</examplecapture>
[Dependent]
public class CardsFromAmong : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Prop(Qualifiers), Plural("card"), Prop(Bound), "from", Prop(Among)];

    public OneOf<UpToCount, Quantity?> Count { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
    [Optional]
    public OneOf<ManaValueLimit, WithQualifier> Bound { get; set; }
    public AmongThem Among { get; set; }
}
