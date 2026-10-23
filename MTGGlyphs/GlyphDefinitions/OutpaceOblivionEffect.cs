namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Outpace Oblivion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outpace Oblivion</exampledoc>
[Dependent]
public class OutpaceOblivionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals 2 damage to each player who doesn't have max speed"];
}
