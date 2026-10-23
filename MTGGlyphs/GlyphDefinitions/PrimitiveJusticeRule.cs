namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Primitive Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Primitive Justice</exampledoc>
[TokenizationOrder(-1)]
public class PrimitiveJusticeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell, you may pay {1}{r} and/or {1}{g} any number of times"];
}
