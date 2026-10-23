namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Game Preserve, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Game Preserve</exampledoc>
[TokenizationOrder(-1)]
public class GamePreserveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if all cards revealed this way are creature cards, put those cards onto the battlefield under their owners' control"];
}
