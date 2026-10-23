namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cathedral Membrane, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cathedral Membrane</exampledoc>
[Dependent]
public class CathedralMembraneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals 6 damage to each creature it blocked this combat"];
}
