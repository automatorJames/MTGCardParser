namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Culling Scales, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Culling Scales</exampledoc>
[Dependent]
public class CullingScalesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more permanents are tied for lowest, target any one of them"];
}
