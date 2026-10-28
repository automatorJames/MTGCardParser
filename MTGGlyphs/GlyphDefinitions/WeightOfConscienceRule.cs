namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Weight of Conscience, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Weight of Conscience</exampledoc>
[TokenizationOrder(-1)]
public class WeightOfConscienceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap two untapped creatures you control that share a creature type: exile enchanted creature"];
}
