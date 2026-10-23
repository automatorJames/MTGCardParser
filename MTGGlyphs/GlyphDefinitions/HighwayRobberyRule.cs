namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Highway Robbery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Highway Robbery</exampledoc>
[TokenizationOrder(-1)]
public class HighwayRobberyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may discard a card or sacrifice a land"];
}
