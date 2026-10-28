namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mages' Contest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mages' Contest</exampledoc>
[TokenizationOrder(-1)]
public class MagesContestRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the high bidder loses life equal to the high bid"];
}
