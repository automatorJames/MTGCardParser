namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Desolation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desolation</exampledoc>
[TokenizationOrder(-1)]
public class Desolation2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals 2 damage to each player who sacrificed a plains this way"];
}
