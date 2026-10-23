namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gelatinous Cube, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gelatinous Cube</exampledoc>
[Dependent]
public class GelatinousCubeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put target creature card with mana value x exiled with this creature into its owner's graveyard"];
}
