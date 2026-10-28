namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Timesifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Timesifter</exampledoc>
[TokenizationOrder(-1)]
public class TimesifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the player who exiled the card with the greatest mana value takes an extra turn after this one"];
}
