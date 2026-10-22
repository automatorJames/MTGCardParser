namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the top card of your graveyard", "the top creature card of your graveyard": the most recent card (of a type) put into a graveyard, where order matters.</summary>
/// <exampledoc>Necratog</exampledoc>
/// <examplecapture>the top creature card of your graveyard</examplecapture>
[Dependent]
public class TopCardOfGraveyard : Glyph
{
    public override Nib[] Nibs => ["the top", Prop(CardType), "card of", Prop(Whose), "graveyard"];

    [Optional]
    public CardType? CardType { get; set; }
    public Whose Whose { get; set; }
}
