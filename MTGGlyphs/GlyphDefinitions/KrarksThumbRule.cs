namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Krark's Thumb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Krark's Thumb</exampledoc>
[TokenizationOrder(-1)]
public class KrarksThumbRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would flip a coin, instead flip two coins and ignore one"];
}
