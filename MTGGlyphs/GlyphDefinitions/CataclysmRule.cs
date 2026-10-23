namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cataclysm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cataclysm</exampledoc>
[TokenizationOrder(-1)]
public class CataclysmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player chooses from among the permanents they control an artifact, a creature, an enchantment, and a land, then sacrifices the rest"];
}
