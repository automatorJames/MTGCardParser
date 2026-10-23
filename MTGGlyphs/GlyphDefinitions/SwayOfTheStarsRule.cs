namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sway of the Stars, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sway of the Stars</exampledoc>
[TokenizationOrder(-1)]
public class SwayOfTheStarsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player shuffles their hand, graveyard, and all permanents they own into their library, then draws seven cards"];
}
