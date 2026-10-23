namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sea Gate Stormcaller, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sea Gate Stormcaller</exampledoc>
[Dependent]
public class SeaGateStormcallerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy the next instant or sorcery spell with mana value 2 or less you cast this turn when you cast it"];
}
