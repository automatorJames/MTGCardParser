namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"three elementals", "two creatures": a number of things of one kind, as beheld.</summary>
/// <exampledoc>Kindle the Inner Flame</exampledoc>
/// <examplecapture>three elementals</examplecapture>
[Dependent]
public class CountedKinds : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Plural(Prop(Kind))];

    public Quantity Count { get; set; }
    public PermanentKind Kind { get; set; }
}
