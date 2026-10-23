namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Metamorphic Alteration, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Metamorphic Alteration</exampledoc>
[TokenizationOrder(-1)]
public class MetamorphicAlterationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature is a copy of the chosen creature"];
}
