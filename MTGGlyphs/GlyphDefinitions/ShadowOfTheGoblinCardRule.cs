namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shadow of the Goblin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shadow of the Goblin</exampledoc>
[TokenizationOrder(-1)]
public class ShadowOfTheGoblinCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["undying vengeance — whenever you play a land or cast a spell from anywhere other than your hand, this enchantment deals 1 damage to each opponent"];
}
