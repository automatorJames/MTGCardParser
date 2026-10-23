namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rowan's Grim Search, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rowan's Grim Search</exampledoc>
/// <examplecapture>if this spell was bargained, look at the top four cards of your library, then put up to two of them back on top of your library in any order and the rest into your graveyard</examplecapture>
[TokenizationOrder(-1)]
public class RowansGrimSearchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was bargained, look at the top four cards of your library, then put up to two of them back on top of your library in any order and the rest into your graveyard"];
}
