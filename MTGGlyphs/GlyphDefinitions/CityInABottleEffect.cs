namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of City in a Bottle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>City in a Bottle</exampledoc>
/// <examplecapture>their controllers sacrifice them</examplecapture>
[Dependent]
public class CityInABottleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["their controllers sacrifice them"];
}
