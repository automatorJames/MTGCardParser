namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Key to the Side-Door, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Key to the Side-Door</exampledoc>
[TokenizationOrder(-1)]
public class KeyToTheSideDoorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}, {t}, discard a legendary card with the same name as a legendary permanent you control: draw two cards"];
}
