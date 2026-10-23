namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tempest Efreet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tempest Efreet</exampledoc>
[TokenizationOrder(-1)]
public class TempestEfreetCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange ownership of the revealed card and", Nib.This];
}
