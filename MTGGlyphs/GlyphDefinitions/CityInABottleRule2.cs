namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of City in a Bottle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>City in a Bottle</exampledoc>
[TokenizationOrder(-1)]
public class CityInABottleRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't cast spells or play lands with a name originally printed in the arabian nights expansion"];
}
