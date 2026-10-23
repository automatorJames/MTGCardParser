namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychogenic Probe, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychogenic Probe</exampledoc>
[TokenizationOrder(-1)]
public class PsychogenicProbeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a spell or ability causes a player to shuffle their library, this artifact deals 2 damage to that player"];
}
