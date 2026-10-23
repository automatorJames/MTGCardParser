namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mycosynth Lattice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mycosynth Lattice</exampledoc>
[TokenizationOrder(-1)]
public class MycosynthLatticeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all cards that aren't on the battlefield, spells, and permanents are colorless"];
}
