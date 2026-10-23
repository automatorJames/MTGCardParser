namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ydwen Efreet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ydwen Efreet</exampledoc>
[TokenizationOrder(-1)]
public class YdwenEfreetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures it was blocking that had become blocked by only this creature this combat become unblocked"];
}
