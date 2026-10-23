namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Verdeloth the Ancient, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Verdeloth the Ancient</exampledoc>
[TokenizationOrder(-1)]
public class VerdelothTheAncientRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["saproling creatures and other treefolk creatures get +1/+1"];
}
