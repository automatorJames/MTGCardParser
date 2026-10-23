namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of All Will Be One, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>All Will Be One</exampledoc>
/// <examplecapture>this enchantment deals that much damage to target opponent, creature an opponent controls, or planeswalker an opponent controls</examplecapture>
[Dependent]
public class AllWillBeOneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals that much damage to target opponent, creature an opponent controls, or planeswalker an opponent controls"];
}
