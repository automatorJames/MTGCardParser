namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spy Network, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spy Network</exampledoc>
[TokenizationOrder(-1)]
public class SpyNetworkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at target player's hand, the top card of that player's library, and any face-down creatures they control"];
}
