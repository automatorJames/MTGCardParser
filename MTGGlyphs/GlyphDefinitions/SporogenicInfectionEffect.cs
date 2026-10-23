namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sporogenic Infection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sporogenic Infection</exampledoc>
[Dependent]
public class SporogenicInfectionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player sacrifices a creature of their choice other than enchanted creature"];
}
