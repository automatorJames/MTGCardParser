namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sungold Sentinel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sungold Sentinel</exampledoc>
[TokenizationOrder(-1)]
public class SungoldSentinelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gains hexproof from that color until end of turn and can't be blocked by creatures of that color this turn"];
}
