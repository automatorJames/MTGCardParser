namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Koth of the Hammer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Koth of the Hammer</exampledoc>
[TokenizationOrder(-1)]
public class KothOfTheHammerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["'\""];
}
