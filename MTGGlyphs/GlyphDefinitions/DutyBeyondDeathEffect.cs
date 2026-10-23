namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Duty Beyond Death, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Duty Beyond Death</exampledoc>
[Dependent]
public class DutyBeyondDeathEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage and effects that say \"destroy\" don't destroy those creatures"];
}
