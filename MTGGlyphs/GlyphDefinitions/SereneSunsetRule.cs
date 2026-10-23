namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Serene Sunset, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serene Sunset</exampledoc>
[TokenizationOrder(-1)]
public class SereneSunsetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage x target creatures would deal this turn"];
}
