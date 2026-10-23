namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grab the Prize, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grab the Prize</exampledoc>
[TokenizationOrder(-1)]
public class GrabThePrizeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the discarded card wasn't a land card,", Nib.This, "deals 2 damage to each opponent"];
}
