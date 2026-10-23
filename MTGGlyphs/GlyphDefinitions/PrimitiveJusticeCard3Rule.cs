namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Primitive Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Primitive Justice</exampledoc>
[TokenizationOrder(-1)]
public class PrimitiveJusticeCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each additional {1}{r} you paid, destroy another target artifact"];
}
