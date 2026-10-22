namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A verb and the buff or buffs it gives: "gets +1/+1", "has flying", "loses first strike or swampwalk", "lose flying and islandwalk", "gets an additional +1/+0" (on top of a buff given already).</summary>
/// <exampledoc>Blessing</exampledoc>
/// <examplecapture>gets +1/+1</examplecapture>
[Dependent]
public class GainedOrLostBuff : Glyph
{
    public PermanentVerb PermanentVerb { get; set; }
    [Optional]
    [RegexPattern("an additional")]
    public bool? Additional { get; set; }
    public OneOf<ManyOf<Buff>, Buff> Buff { get; set; }
}