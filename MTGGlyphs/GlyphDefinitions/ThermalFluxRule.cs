namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thermal Flux, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thermal Flux</exampledoc>
[TokenizationOrder(-1)]
public class ThermalFluxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• target nonsnow permanent becomes snow until end of turn"];
}
