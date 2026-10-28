namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Head Games, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Head Games</exampledoc>
[TokenizationOrder(-1)]
public class HeadGamesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent puts the cards from their hand on top of their library"];
}
