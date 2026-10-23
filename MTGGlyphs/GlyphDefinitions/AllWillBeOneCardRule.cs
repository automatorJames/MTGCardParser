namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of All Will Be One, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>All Will Be One</exampledoc>
[TokenizationOrder(-1)]
public class AllWillBeOneCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you put one or more counters on a permanent or player, this enchantment deals that much damage to target opponent, creature an opponent controls, or planeswalker an opponent controls"];
}
