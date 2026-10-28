namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Don & Raph, Hard Science, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Don &amp; Raph, Hard Science</exampledoc>
[TokenizationOrder(-1)]
public class DonRaphHardScienceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever don & raph attack, the next noncreature spell you cast this turn has affinity for artifacts"];
}
