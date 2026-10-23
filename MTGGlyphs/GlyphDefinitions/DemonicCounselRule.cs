namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Demonic Counsel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Demonic Counsel</exampledoc>
/// <examplecapture>delirium — if there are four or more card types among cards in your graveyard, instead search your library for any card, put it into your hand, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class DemonicCounselRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["delirium — if there are four or more card types among cards in your graveyard, instead search your library for any card, put it into your hand, then shuffle"];
}
