namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dig Up, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dig Up</exampledoc>
[TokenizationOrder(-1)]
public class DigUpRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a [basic land] card, [reveal it,] put it into your hand, then shuffle"];
}
