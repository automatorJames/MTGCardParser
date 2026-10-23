namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Don & Raph, Hard Science, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Don &amp; Raph, Hard Science</exampledoc>
[Dependent]
public class DonRaphHardScienceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next noncreature spell you cast this turn has affinity for artifacts"];
}
