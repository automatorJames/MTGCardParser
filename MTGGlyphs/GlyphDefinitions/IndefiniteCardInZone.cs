namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Any one card of a kind in a zone other than the battlefield, not targeted: "a creature card from your graveyard", "a card from a graveyard", "a green card from your hand", "a dragon permanent card from your graveyard", "a noncreature, nonland card from your hand", "another creature card from your graveyard" - as exiled, chosen or returned.</summary>
/// <exampledoc>Corpse Lunge</exampledoc>
/// <examplecapture>a creature card from your graveyard</examplecapture>
[Dependent]
public class IndefiniteCardInZone : Glyph
{
    public override Nib[] Nibs => [Pattern("another|an?"), Prop(Qualifiers), Prop(Card)];

    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
    public CardOutsideBattlefield Card { get; set; }
}
