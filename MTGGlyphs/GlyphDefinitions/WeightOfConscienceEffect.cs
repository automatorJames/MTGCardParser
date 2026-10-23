namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Weight of Conscience, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Weight of Conscience</exampledoc>
[Dependent]
public class WeightOfConscienceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile enchanted creature"];
}
