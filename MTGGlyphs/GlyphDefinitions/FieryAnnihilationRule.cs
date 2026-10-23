namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fiery Annihilation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Annihilation</exampledoc>
[TokenizationOrder(-1)]
public class FieryAnnihilationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile up to one target equipment attached to that creature"];
}
