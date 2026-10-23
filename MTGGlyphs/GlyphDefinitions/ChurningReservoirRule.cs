namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Churning Reservoir, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Churning Reservoir</exampledoc>
/// <examplecapture>activate only if an oil counter was removed from a permanent you controlled this turn or a permanent with an oil counter on it was put into a graveyard this turn</examplecapture>
[TokenizationOrder(-1)]
public class ChurningReservoirRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if an oil counter was removed from a permanent you controlled this turn or a permanent with an oil counter on it was put into a graveyard this turn"];
}
