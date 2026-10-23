namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Storage Matrix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Storage Matrix</exampledoc>
[TokenizationOrder(-1)]
public class StorageMatrixRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this artifact is untapped, each player chooses artifact, creature, or land during their untap step"];
}
