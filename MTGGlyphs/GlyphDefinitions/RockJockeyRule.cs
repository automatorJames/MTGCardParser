namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rock Jockey, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rock Jockey</exampledoc>
[TokenizationOrder(-1)]
public class RockJockeyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't cast", Nib.This, "if you've played a land this turn"];
}
