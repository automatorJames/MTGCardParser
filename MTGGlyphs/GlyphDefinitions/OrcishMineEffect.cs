namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Orcish Mine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Orcish Mine</exampledoc>
[Dependent]
public class OrcishMineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy enchanted land and this aura deals 2 damage to that land's controller"];
}
