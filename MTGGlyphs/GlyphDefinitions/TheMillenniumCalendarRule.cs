namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Millennium Calendar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Millennium Calendar</exampledoc>
[TokenizationOrder(-1)]
public class TheMillenniumCalendarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when there are 1,000 or more time counters on", Nib.This, ", sacrifice it and each opponent loses 1,000 life"];
}
