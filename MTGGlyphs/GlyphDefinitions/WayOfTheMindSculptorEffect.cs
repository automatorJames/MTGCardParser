namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Way of the Mind Sculptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Way of the Mind Sculptor</exampledoc>
[Dependent]
public class WayOfTheMindSculptorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you removed two or more loyalty counters to activate it, draw a card"];
}
