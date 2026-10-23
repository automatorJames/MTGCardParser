namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Storage Matrix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Storage Matrix</exampledoc>
[TokenizationOrder(-1)]
public class StorageMatrixCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player can untap only permanents of the chosen type this step"];
}
