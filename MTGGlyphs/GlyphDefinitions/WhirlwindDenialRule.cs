namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Whirlwind Denial, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whirlwind Denial</exampledoc>
[TokenizationOrder(-1)]
public class WhirlwindDenialRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each spell and ability your opponents control, counter it unless its controller pays {4}"];
}
