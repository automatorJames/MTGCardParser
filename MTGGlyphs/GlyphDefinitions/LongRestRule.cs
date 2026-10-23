namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Long Rest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Long Rest</exampledoc>
[TokenizationOrder(-1)]
public class LongRestRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if eight or more cards were returned to your hand this way, your life total becomes equal to your starting life total"];
}
