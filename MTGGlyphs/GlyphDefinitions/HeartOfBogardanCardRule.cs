namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Heart of Bogardan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heart of Bogardan</exampledoc>
[TokenizationOrder(-1)]
public class HeartOfBogardanCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when a player doesn't pay this enchantment's cumulative upkeep, this enchantment deals x damage to target player or planeswalker and each creature that player or that planeswalker's controller controls, where x is twice the number of age counters on this enchantment minus 2"];
}
