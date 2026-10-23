namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Novijen Sages, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Novijen Sages</exampledoc>
[TokenizationOrder(-1)]
public class NovijenSagesCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}, remove two +1/+1 counters from among creatures you control: draw a card"];
}
