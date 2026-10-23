namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Each card of a kind in a zone, as a subject: "each creature card in your hand", "each instant and sorcery card in your graveyard", "each sliver card in each player's hand", "each nonland card in your graveyard". Tried before <see cref="EachObject"/>, which would stop at "each creature".</summary>
/// <exampledoc>Satoru Umezawa</exampledoc>
/// <examplecapture>each creature card in your hand</examplecapture>
[Dependent]
public class EachCardInZone : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Cards)];

    public CardsInZone Cards { get; set; }
}
