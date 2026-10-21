namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of counters of a kind: "a doom counter", "three +1/+1 counters", "up to x +1/+0 counters", "a -0/-1 counter", "three or more +1/+1 counters", or counters named by whose or how many they are: "its +1/+1 counters", "all counters", "that many charge counters". Power/toughness counters other than the named ones are written as a power/toughness change.</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>three +1/+1 counters</examplecapture>
[Dependent]
public class Counters : Glyph
{
    public override Nib[] Nibs => [Prop(UpTo), Prop(Quantity), Prop(OrMore), Prop(Kind), Plural("counter")];

    [RegexPattern("up to")]
    public bool UpTo { get; set; }
    public OneOf<Quantity?, CounterSet?> Quantity { get; set; }
    [RegexPattern("or more")]
    public bool OrMore { get; set; }
    [Optional]
    public OneOf<CounterType?, PowerToughnessMod> Kind { get; set; }
}
