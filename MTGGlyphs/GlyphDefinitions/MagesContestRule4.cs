namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mages' Contest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mages' Contest</exampledoc>
[TokenizationOrder(-1)]
public class MagesContestRule4 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win the bidding, counter that spell"];
}
