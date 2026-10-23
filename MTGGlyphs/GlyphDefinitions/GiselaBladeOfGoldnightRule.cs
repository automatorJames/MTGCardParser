namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gisela, Blade of Goldnight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gisela, Blade of Goldnight</exampledoc>
[TokenizationOrder(-1)]
public class GiselaBladeOfGoldnightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a source would deal damage to you or a permanent you control, prevent half that damage, rounded up"];
}
