namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nature's Blessing, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nature's Blessing</exampledoc>
[Dependent]
public class NaturesBlessingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on target creature or that creature gains banding, first strike, or trample"];
}
