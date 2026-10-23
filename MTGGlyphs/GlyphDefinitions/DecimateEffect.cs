namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Decimate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Decimate</exampledoc>
[Dependent]
public class DecimateEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't cast this spell unless you have legal choices for all its targets"];
}
