namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Barren Glory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Barren Glory</exampledoc>
/// <examplecapture>if you control no permanents other than this enchantment and have no cards in hand, you win the game</examplecapture>
[Dependent]
public class BarrenGloryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control no permanents other than this enchantment and have no cards in hand, you win the game"];
}
