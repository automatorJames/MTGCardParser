namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Addle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Addle</exampledoc>
[TokenizationOrder(-1)]
public class AddleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals their hand and you choose a card of that color from it"];
}
