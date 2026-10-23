namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fight or Flight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fight or Flight</exampledoc>
[TokenizationOrder(-1)]
public class FightOrFlightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only creatures in the pile of their choice can attack this turn"];
}
