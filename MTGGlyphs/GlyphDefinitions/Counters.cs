namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Some number of counters of one kind: "a +1/+1 counter", "x +1/+0 counters", "a vitality counter".</summary>
[Dependent]
public class Counters : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(Kind), Plural("counter")];

    public Quantity Quantity { get; set; }
    public OneOf<PowerToughnessMod, CounterType?> Kind { get; set; }
}
