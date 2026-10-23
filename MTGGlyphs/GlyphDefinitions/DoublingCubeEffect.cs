namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Doubling Cube, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doubling Cube</exampledoc>
[Dependent]
public class DoublingCubeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double the amount of each type of unspent mana you have"];
}
