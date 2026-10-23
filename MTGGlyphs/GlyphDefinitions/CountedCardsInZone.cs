namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of cards of a kind in a zone other than the battlefield, not targeted: "two cards from your graveyard", "x creature cards from your graveyard", "two green cards from your hand", "up to two creature cards from your graveyard", "three other cards from your graveyard" (escape) - as exiled, put, returned or revealed. "other" excludes the card itself.</summary>
/// <exampledoc>Psychatog</exampledoc>
/// <examplecapture>two cards from your graveyard</examplecapture>
[Dependent]
public class CountedCardsInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Prop(Other), Prop(Qualifiers), Prop(Card)];

    public OneOf<UpToCount, Quantity?> Count { get; set; }
    [Optional]
    [RegexPattern("other")]
    public bool? Other { get; set; }
    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
    public CardOutsideBattlefield Card { get; set; }
}
