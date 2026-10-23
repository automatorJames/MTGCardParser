namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Teachings of the Archaics, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teachings of the Archaics</exampledoc>
[TokenizationOrder(-1)]
public class TeachingsOfTheArchaicsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw three cards instead if an opponent has at least four more cards in hand than you"];
}
