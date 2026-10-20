namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mill's reminder text, for any number of cards: "to mill a card, put the top card of your library into your graveyard", "to mill three cards, put the top three cards of your library into your graveyard".</summary>
/// <exampledoc>Deranged Assistant</exampledoc>
/// <examplecapture>to mill a card, put the top card of your library into your graveyard</examplecapture>
public class MillRule : Glyph
{
    public override Nib[] Nibs => ["to mill", Prop(Count), Plural("card"), ", put the top", Prop(TopCount), Plural("card"), "of your library into your graveyard"];

    public Quantity Count { get; set; }
    [Optional]
    public Quantity? TopCount { get; set; }
}
