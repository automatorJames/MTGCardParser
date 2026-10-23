namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spatial Binding, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spatial Binding</exampledoc>
[Dependent]
public class SpatialBindingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next upkeep, target permanent can't phase out"];
}
