namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Acidic Dagger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Acidic Dagger</exampledoc>
[Dependent]
public class AcidicDaggerCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice this artifact"];
}
