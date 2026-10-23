namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Finest Hour, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Finest Hour</exampledoc>
[Dependent]
public class FinestHourEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first combat phase of the turn, untap that creature"];
}
