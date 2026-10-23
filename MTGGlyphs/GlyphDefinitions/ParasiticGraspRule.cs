namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Parasitic Grasp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parasitic Grasp</exampledoc>
[TokenizationOrder(-1)]
public class ParasiticGraspRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 3 damage to target [human] creature"];
}
