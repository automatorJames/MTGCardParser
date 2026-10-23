namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rankle and Torbran, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rankle and Torbran</exampledoc>
[TokenizationOrder(-1)]
public class RankleAndTorbranRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• if a source would deal damage to a player or battle this turn, it deals that much damage plus 2 instead"];
}
