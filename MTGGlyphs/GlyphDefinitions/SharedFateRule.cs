namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shared Fate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shared Fate</exampledoc>
[TokenizationOrder(-1)]
public class SharedFateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a player would draw a card, that player exiles the top card of one of their opponents' libraries face down instead"];
}
