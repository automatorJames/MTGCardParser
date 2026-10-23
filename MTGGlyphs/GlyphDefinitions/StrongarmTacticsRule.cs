namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Strongarm Tactics, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Strongarm Tactics</exampledoc>
[TokenizationOrder(-1)]
public class StrongarmTacticsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player who didn't discard a creature card this way loses 4 life"];
}
