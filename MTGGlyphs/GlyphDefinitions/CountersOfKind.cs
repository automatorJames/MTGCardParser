namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters of a kind, uncounted: "+1/+1 counters", "ice counters", "counters" - what a group of permanents carries ("creatures with ice counters on them").</summary>
/// <exampledoc>Rimescale Dragon</exampledoc>
/// <examplecapture>ice counters</examplecapture>
[Dependent]
public class CountersOfKind : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "counters"];

    [Optional]
    public OneOf<CounterType?, PowerToughnessMod> Kind { get; set; }
}
