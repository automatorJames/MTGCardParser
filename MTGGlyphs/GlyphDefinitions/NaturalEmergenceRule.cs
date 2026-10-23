namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Natural Emergence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Natural Emergence</exampledoc>
[TokenizationOrder(-1)]
public class NaturalEmergenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lands you control are 2/2 creatures with first strike"];
}
