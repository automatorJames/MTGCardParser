namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hydro-Man, Fluid Felon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hydro-Man, Fluid Felon</exampledoc>
[TokenizationOrder(-1)]
public class HydroManFluidFelonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(he's not a creature during that time"];
}
