namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Check for Traps, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Check for Traps</exampledoc>
[TokenizationOrder(-1)]
public class CheckForTrapsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an instant card or a card with flash is exiled this way, they lose 1 life"];
}
