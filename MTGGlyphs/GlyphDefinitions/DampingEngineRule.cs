namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Damping Engine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Damping Engine</exampledoc>
[TokenizationOrder(-1)]
public class DampingEngineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a player who controls more permanents than each other player can't play lands or cast artifact, creature, or enchantment spells"];
}
