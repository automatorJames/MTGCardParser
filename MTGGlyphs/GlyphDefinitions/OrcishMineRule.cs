namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Orcish Mine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Orcish Mine</exampledoc>
[TokenizationOrder(-1)]
public class OrcishMineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when the last ore counter is removed from this aura, destroy enchanted land and this aura deals 2 damage to that land's controller"];
}
