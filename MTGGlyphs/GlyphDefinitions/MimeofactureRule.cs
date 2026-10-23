namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mimeofacture, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mimeofacture</exampledoc>
[TokenizationOrder(-1)]
public class MimeofactureRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search that player's library for a card with the same name and put it onto the battlefield under your control"];
}
