namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Unagi of Kyoshi Island, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Unagi of Kyoshi Island</exampledoc>
[TokenizationOrder(-1)]
public class TheUnagiOfKyoshiIslandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they can tap their artifacts and creatures to help"];
}
