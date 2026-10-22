namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"two targets", "up to three targets", "x targets": several targets each of which can be a player or a permanent, as in "each of up to two targets".</summary>
/// <exampledoc>Fall of the Titans</exampledoc>
/// <examplecapture>up to two targets</examplecapture>
[Dependent]
public class CountedAnyTargets : Glyph
{
    public override Nib[] Nibs => [Prop(UpTo), Prop(Count), "targets"];

    [RegexPattern("up to")]
    public bool UpTo { get; set; }
    public Quantity Count { get; set; }
}
