namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tempest Efreet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tempest Efreet</exampledoc>
[TokenizationOrder(-1)]
public class TempestEfreetCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this change in ownership is permanent"];
}
