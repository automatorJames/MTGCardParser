namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Desert Were-Worm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desert Were-Worm</exampledoc>
[TokenizationOrder(-1)]
public class DesertWereWormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you attack with creatures with total power 12 or greater for the first time each turn, untap all attacking creatures"];
}
