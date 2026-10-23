namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Golden Demise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Golden Demise</exampledoc>
[TokenizationOrder(-1)]
public class GoldenDemiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have the city's blessing, instead only creatures your opponents control get -2/-2 until end of turn"];
}
