namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of City in a Bottle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>City in a Bottle</exampledoc>
[TokenizationOrder(-1)]
public class CityInABottleCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more other nontoken permanents with a name originally printed in the arabian nights expansion are on the battlefield, their controllers sacrifice them"];
}
