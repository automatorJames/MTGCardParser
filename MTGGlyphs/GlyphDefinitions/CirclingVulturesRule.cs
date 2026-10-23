namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Circling Vultures, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Circling Vultures</exampledoc>
[TokenizationOrder(-1)]
public class CirclingVulturesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may discard this card any time you could cast an instant"];
}
