namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Filigree Sylex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Filigree Sylex</exampledoc>
[TokenizationOrder(-1)]
public class TheFiligreeSylexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{t}, remove ten oil counters from among permanents you control and sacrifice", Nib.This, ": it deals 10 damage to any target"];
}
