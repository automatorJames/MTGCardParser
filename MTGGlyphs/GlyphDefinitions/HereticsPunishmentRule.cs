namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Heretic's Punishment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heretic's Punishment</exampledoc>
[TokenizationOrder(-1)]
public class HereticsPunishmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals damage to that permanent or player equal to the greatest mana value among the milled cards"];
}
