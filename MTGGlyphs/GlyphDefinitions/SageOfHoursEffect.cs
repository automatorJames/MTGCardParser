namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sage of Hours, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sage of Hours</exampledoc>
[Dependent]
public class SageOfHoursEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each five counters removed this way, take an extra turn after this one"];
}
