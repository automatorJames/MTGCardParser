namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Heat Stroke, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heat Stroke</exampledoc>
[Dependent]
public class HeatStrokeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each creature that blocked or was blocked this turn"];
}
