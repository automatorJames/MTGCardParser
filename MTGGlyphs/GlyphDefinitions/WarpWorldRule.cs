namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Warp World, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Warp World</exampledoc>
/// <examplecapture>each player puts all artifact, creature, and land cards revealed this way onto the battlefield, then does the same for enchantment cards, then puts all cards revealed this way that weren't put onto the battlefield on the bottom of their library</examplecapture>
[TokenizationOrder(-1)]
public class WarpWorldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player puts all artifact, creature, and land cards revealed this way onto the battlefield, then does the same for enchantment cards, then puts all cards revealed this way that weren't put onto the battlefield on the bottom of their library"];
}
