namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Light Up the Night, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Light Up the Night</exampledoc>
[TokenizationOrder(-1)]
public class LightUpTheNightRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flashback—{3}{r}, remove x loyalty counters from among planeswalkers you control"];
}
