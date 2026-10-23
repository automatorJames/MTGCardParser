namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The rules note after a damage-triggered counter ability: "it must survive the damage to get the counter" - the creature gets the counters only if it's still on the battlefield when the ability resolves.</summary>
/// <exampledoc>Snapping Sailback</exampledoc>
/// <examplecapture>it must survive the damage to get the counter</examplecapture>
[Dependent]
public class MustSurviveForCounters : Glyph
{
    public override Nib[] Nibs => [Alt("it", "she", "he"), "must survive", Opt("the damage"), "to get the", Plural("counter")];
}
