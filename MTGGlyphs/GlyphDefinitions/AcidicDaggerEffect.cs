namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Acidic Dagger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Acidic Dagger</exampledoc>
[Dependent]
public class AcidicDaggerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever target creature deals combat damage to a non-wall creature this turn, destroy that non-wall creature"];
}
