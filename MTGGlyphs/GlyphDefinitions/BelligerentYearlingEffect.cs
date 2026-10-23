namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Belligerent Yearling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Belligerent Yearling</exampledoc>
[Dependent]
public class BelligerentYearlingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature's base power become equal to that creature's power until end of turn"];
}
