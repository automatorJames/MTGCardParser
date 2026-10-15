namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The parenthetical count after energy: "(two energy counters)".</summary>
/// <exampledoc>Aether Chaser</exampledoc>
/// <examplecapture>(two energy counters)</examplecapture>
[Dependent]
public class EnergyNote : Glyph
{
    public override Nib[] Nibs => [Pattern(@"\((an|two|three|four|five|six|seven|eight) energy counters?\)")];
}
