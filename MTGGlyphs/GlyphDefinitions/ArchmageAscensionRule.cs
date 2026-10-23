namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Archmage Ascension, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Archmage Ascension</exampledoc>
/// <examplecapture>as long as this enchantment has six or more quest counters on it, if you would draw a card, you may instead search your library for a card, put that card into your hand, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class ArchmageAscensionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this enchantment has six or more quest counters on it, if you would draw a card, you may instead search your library for a card, put that card into your hand, then shuffle"];
}
