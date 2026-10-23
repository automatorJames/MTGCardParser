namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reclaim the Wastes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reclaim the Wastes</exampledoc>
[TokenizationOrder(-1)]
public class ReclaimTheWastesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was kicked, search your library for two basic land cards instead of one"];
}
