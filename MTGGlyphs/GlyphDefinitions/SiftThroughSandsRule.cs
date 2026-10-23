namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sift Through Sands, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sift Through Sands</exampledoc>
/// <examplecapture>if you've cast a spell named peer through depths and a spell named reach through mists this turn, you may search your library for a card named the unspeakable, put it onto the battlefield, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class SiftThroughSandsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you've cast a spell named peer through depths and a spell named reach through mists this turn, you may search your library for a card named the unspeakable, put it onto the battlefield, then shuffle"];
}
