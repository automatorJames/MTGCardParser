namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Life and Limb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Life and Limb</exampledoc>
[TokenizationOrder(-1)]
public class LifeAndLimbRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all forests and all saprolings are 1/1 green saproling creatures and forest lands in addition to their other types"];
}
