namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of cards of a kind in a zone other than the battlefield, not targeted: "two cards from your graveyard", "x creature cards from your graveyard", "two green cards from your hand" - as exiled, put or revealed.</summary>
/// <exampledoc>Psychatog</exampledoc>
/// <examplecapture>two cards from your graveyard</examplecapture>
[Dependent]
public class CountedCardsInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Prop(Qualifiers), Prop(Card)];

    public Quantity Count { get; set; }
    [Optional]
    public OneOf<CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
    public CardOutsideBattlefield Card { get; set; }
}
