namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Takklemaggot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Takklemaggot</exampledoc>
[TokenizationOrder(-1)]
public class TakklemaggotRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it loses \"enchant creature\" and gains \"at the beginning of that player's upkeep, this enchantment deals 1 damage to that player"];
}
