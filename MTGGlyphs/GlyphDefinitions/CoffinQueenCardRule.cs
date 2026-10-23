namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Coffin Queen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coffin Queen</exampledoc>
[TokenizationOrder(-1)]
public class CoffinQueenCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature becomes untapped or you lose control of this creature, exile that creature"];
}
