namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elemental Expressionist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elemental Expressionist</exampledoc>
[TokenizationOrder(-1)]
public class ElementalExpressionistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(each instance of that ability triggers separately"];
}
