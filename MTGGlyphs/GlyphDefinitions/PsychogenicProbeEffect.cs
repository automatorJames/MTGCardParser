namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Psychogenic Probe, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychogenic Probe</exampledoc>
[Dependent]
public class PsychogenicProbeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this artifact deals 2 damage to that player"];
}
