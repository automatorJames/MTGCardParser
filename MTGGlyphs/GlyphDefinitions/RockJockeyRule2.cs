namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rock Jockey, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rock Jockey</exampledoc>
[TokenizationOrder(-1)]
public class RockJockeyRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't play lands if this creature was cast this turn"];
}
