namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shadow of the Goblin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shadow of the Goblin</exampledoc>
/// <examplecapture>this enchantment deals 1 damage to each opponent</examplecapture>
[Dependent]
public class ShadowOfTheGoblinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals 1 damage to each opponent"];
}
