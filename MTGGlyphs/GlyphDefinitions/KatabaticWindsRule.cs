namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Katabatic Winds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Katabatic Winds</exampledoc>
[TokenizationOrder(-1)]
public class KatabaticWindsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures with flying can't attack or block, and their activated abilities with {t} in their costs can't be activated"];
}
